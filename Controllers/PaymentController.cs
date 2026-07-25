using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models.Sales;
using BioGamaEcuador.Services;
using BioGamaEcuador.Services.Payments;

namespace BioGamaEcuador.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly AppDbContext _context;
    private readonly PayPhoneApiLinkService _payPhoneService;
    private readonly PayPalService _payPal;
    private readonly IInventoryService _inventory;
    private readonly IEmailService _email;
    private readonly IAuditService _audit;

    public PaymentController(
        AppDbContext context,
        PayPhoneApiLinkService payPhoneService,
        PayPalService payPal,
        IInventoryService inventory,
        IEmailService email,
        IAuditService audit)
    {
        _context = context;
        _payPhoneService = payPhoneService;
        _payPal = payPal;
        _inventory = inventory;
        _email = email;
        _audit = audit;
    }

    public async Task<IActionResult> CreateLink(Guid orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return NotFound();

        string clientTxId = $"BIO-{Guid.NewGuid():N}"[..15];

        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == order.Id);
        if (payment == null)
        {
            payment = new Payment
            {
                OrderId = order.Id,
                Provider = "PayPhone",
                ExternalId = clientTxId,
                Amount = order.Total,
                Currency = "USD",
                Status = "Pending"
            };
            _context.Payments.Add(payment);
        }
        else
        {
            payment.ExternalId = clientTxId;
            payment.Provider = "PayPhone";
            payment.Amount = order.Total;
            payment.Status = "Pending";
        }
        await _context.SaveChangesAsync();

        try
        {
            var url = await _payPhoneService.CreatePaymentLinkAsync(
                order.Total, clientTxId, order.Id.ToString());
            payment.GatewayResponse = url;
            await _context.SaveChangesAsync();
            return Redirect(url);
        }
        catch (Exception ex)
        {
            TempData["Warning"] = ex.Message + " Puedes marcar el pago como pagado manualmente desde el panel de administrador.";
            return RedirectToAction(nameof(Details), new { id = payment.Id });
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .ThenInclude(o => o.Details)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null) return NotFound();

        return View(payment);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> MarkAsPaid(Guid id)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .ThenInclude(o => o.Details)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null) return NotFound();
        if (payment.Status != "Pending") return RedirectToAction(nameof(Details), new { id });

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            payment.Status = "Approved";
            payment.ConfirmedAt = DateTime.UtcNow;

            var order = payment.Order;
            order.Status = "Confirmed";

            foreach (var detail in order.Details)
            {
                if (detail.PhysicalProductId.HasValue)
                    await _inventory.ConfirmAsync(detail.PhysicalProductId.Value, detail.Quantity, $"Order:{order.Id}", "system");
                else if (detail.CourseId.HasValue)
                    await _inventory.LogConfirmationAsync(detail.CourseId.Value, detail.Quantity, $"Order:{order.Id}", "system");
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            await _audit.LogAsync("PaymentMarkedAsPaid", "Payment", id.ToString(), "Pending", "Approved", User.Identity!.Name ?? "admin", HttpContext.Connection.RemoteIpAddress?.ToString());
            TempData["Success"] = "Pago marcado como pagado correctamente.";
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [AllowAnonymous]
    [HttpPost("api/payments/payphone/webhook")]
    public async Task<IActionResult> Webhook([FromBody] PayPhoneWebhook body)
    {
        if (body == null || string.IsNullOrWhiteSpace(body.ClientTransactionId))
            return BadRequest("Falta clientTransactionId.");

        var status = body.Status ?? body.TransactionStatus ?? "";
        var payment = await _context.Payments
            .Include(p => p.Order).ThenInclude(o => o.Details)
            .FirstOrDefaultAsync(p => p.ExternalId == body.ClientTransactionId);

        if (payment == null) return Ok("not_found");
        if (payment.Status != "Pending") return Ok(payment.Status);

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var approved = status == "APPROVED";
            payment.Status = approved ? "Approved" : "Failed";
            payment.ConfirmedAt = approved ? DateTime.UtcNow : null;

            var order = payment.Order;
            order.Status = approved ? "Confirmed" : "Cancelled";

            foreach (var detail in order.Details)
            {
                if (detail.PhysicalProductId.HasValue)
                {
                    if (approved)
                        await _inventory.ConfirmAsync(detail.PhysicalProductId.Value, detail.Quantity, $"Order:{order.Id}", "system");
                    else
                        await _inventory.ReleaseAsync(detail.PhysicalProductId.Value, detail.Quantity, $"Order:{order.Id}|failed", "system");
                }
                else if (detail.CourseId.HasValue)
                {
                    if (approved)
                        await _inventory.LogConfirmationAsync(detail.CourseId.Value, detail.Quantity, $"Order:{order.Id}", "system");
                    else
                        await _inventory.LogReleaseAsync(detail.CourseId.Value, detail.Quantity, $"Order:{order.Id}|failed", "system");
                }
            }

            if (!approved && order.UserId != null)
            {
                var user = await _context.Users.FindAsync(order.UserId);
                if (user?.Email != null)
                    await _email.SendPaymentFailedAsync(user.Email, order.Id, "Pago rechazado por PayPhone");
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            await _audit.LogAsync("PaymentWebhookReceived", "Payment", payment.Id.ToString(), "Pending", payment.Status, "system", body.ClientTransactionId);
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return Ok(payment.Status);
    }

    [HttpGet("api/payments/paypal/confirm")]
    [HttpPost("api/payments/paypal/confirm")]
    public async Task<IActionResult> ConfirmPayPal([FromForm] string token, [FromQuery] string? token_q)
    {
        token = token ?? token_q ?? "";
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Falta token de PayPal.");

        var capture = await _payPal.CaptureOrderAsync(token);

        var existing = await _context.Payments.FirstOrDefaultAsync(p => p.ExternalId == token);
        if (existing != null && existing.Status == "Approved")
            return RedirectToActionResult(existing.OrderId);

        var payment = existing ?? await _context.Payments
            .Include(p => p.Order).ThenInclude(o => o.Details)
            .FirstOrDefaultAsync(p => p.ExternalId == token);

        if (payment == null)
        {
            var order = await _context.Orders
                .Include(o => o.Details)
                .FirstOrDefaultAsync(o => o.Payment!.ExternalId == token);
            if (order == null) return NotFound("Orden no encontrada.");
            payment = order.Payment;
        }

        if (payment.Status != "Pending")
            return RedirectToActionResult(payment.OrderId);

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = payment.Order;
            if (order == null) return NotFound();

            payment.Status = "Approved";
            payment.ConfirmedAt = DateTime.UtcNow;
            payment.GatewayResponse = capture.RawResponse;
            order.Status = "Confirmed";

            foreach (var detail in order.Details)
            {
                if (detail.CourseId.HasValue)
                {
                    var enrollment = await _context.Enrollments
                        .FirstOrDefaultAsync(e => e.OrderId == order.Id && e.CourseId == detail.CourseId);
                    if (enrollment != null)
                    {
                        enrollment.Status = "Confirmed";
                        var course = await _context.Courses.FindAsync(detail.CourseId);
                        if (course != null)
                        {
                            course.ReservedSeats -= detail.Quantity;
                            course.ConfirmedSeats += detail.Quantity;
                        }
                        await _inventory.LogConfirmationAsync(detail.CourseId.Value, detail.Quantity, $"Enrollment:{enrollment.Id}", "system");
                    }
                }
                else if (detail.PhysicalProductId.HasValue)
                {
                    await _inventory.ConfirmAsync(detail.PhysicalProductId.Value, detail.Quantity, $"Order:{order.Id}", "system");
                }
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            await _audit.LogAsync("PaymentApproved", "Payment", payment.Id.ToString(), "Pending", "Approved", User.Identity!.Name ?? "system", HttpContext.Connection.RemoteIpAddress?.ToString());
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return RedirectToActionResult(payment.OrderId);
    }

    private IActionResult RedirectToActionResult(Guid orderId)
    {
        TempData["Success"] = "Pago confirmado correctamente.";
        return RedirectToAction("Confirmation", "Orders", new { id = orderId });
    }
}

public class PayPhoneWebhook
{
    public string? ClientTransactionId { get; set; }
    public string? Status { get; set; }
    public string? TransactionStatus { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public string? Reference { get; set; }
}
