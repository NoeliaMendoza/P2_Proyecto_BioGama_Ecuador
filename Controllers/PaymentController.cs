using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models.Sales;
using BioGamaEcuador.Services;
using BioGamaEcuador.Services.Payments;

namespace BioGamaEcuador.Controllers;

[Route("api/payments")]
[ApiExplorerSettings(IgnoreApi = true)]
public class PaymentController : Controller
{
    private readonly AppDbContext _context;
    private readonly IInventoryMovementService _inventory;
    private readonly PayPalService _payPal;

    public PaymentController(AppDbContext context, IInventoryMovementService inventory, PayPalService payPal)
    {
        _context = context;
        _inventory = inventory;
        _payPal = payPal;
    }

    [HttpPost("paypal/confirm")]
    public async Task<IActionResult> ConfirmPayPal([FromForm] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Falta token de PayPal.");

        var capture = await _payPal.CaptureOrderAsync(token);

        // Idempotencia: verificar si ya fue procesado
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
            // Validar monto, moneda, orden
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
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return RedirectToActionResult(payment.OrderId);
    }

    [HttpPost("payphone/confirm")]
    public async Task<IActionResult> ConfirmPayPhone([FromBody] PayPhoneWebhook body)
    {
        if (body == null || string.IsNullOrWhiteSpace(body.ClientTransactionId))
            return BadRequest();

        // Idempotencia
        var existing = await _context.Payments
            .Include(p => p.Order).ThenInclude(o => o.Details)
            .FirstOrDefaultAsync(p => p.ExternalId == body.ClientTransactionId);

        if (existing == null) return NotFound();
        if (existing.Status != "Pending")
            return Ok(new { status = existing.Status });

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var approved = body.Status == "APPROVED" || body.TransactionStatus == "APPROVED";
            existing.Status = approved ? "Approved" : "Failed";
            existing.ConfirmedAt = approved ? DateTime.UtcNow : null;
            existing.GatewayResponse = System.Text.Json.JsonSerializer.Serialize(body);

            var order = existing.Order;
            if (order == null) return NotFound();
            order.Status = approved ? "Confirmed" : "Cancelled";

            if (approved)
            {
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
            }
            else
            {
                // Liberar reservas
                foreach (var detail in order.Details)
                {
                    if (detail.CourseId.HasValue)
                    {
                        var course = await _context.Courses.FindAsync(detail.CourseId);
                        if (course != null)
                            course.ReservedSeats -= detail.Quantity;
                        await _inventory.LogReleaseAsync(detail.CourseId.Value, detail.Quantity, $"Order:{order.Id}|payment_failed", "system");
                    }
                    else if (detail.PhysicalProductId.HasValue)
                    {
                        await _inventory.ReleaseAsync(detail.PhysicalProductId.Value, detail.Quantity, $"Order:{order.Id}|payment_failed", "system");
                    }
                }
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return Ok(new { status = existing.Status });
    }

    private IActionResult RedirectToActionResult(Guid orderId)
    {
        TempData["Success"] = "Pago confirmado correctamente.";
        return RedirectToAction("Confirmation", "Orders", new { orderId });
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
