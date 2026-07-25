using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models.Sales;
using BioGamaEcuador.Services;
using BioGamaEcuador.Services.Payments;

namespace BioGamaEcuador.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly AppDbContext _context;
    private readonly IEmailService _email;
    private readonly PayPalService _payPal;
    private readonly PayPhoneApiLinkService _payPhone;

    public OrdersController(AppDbContext context, IEmailService email, PayPalService payPal, PayPhoneApiLinkService payPhone)
    {
        _context = context;
        _email = email;
        _payPal = payPal;
        _payPhone = payPhone;
    }

    private async Task<string> GetUserIdAsync() =>
        (await _context.Users.Where(u => u.UserName == User.Identity!.Name).Select(u => u.Id).FirstOrDefaultAsync())
        ?? throw new UnauthorizedAccessException();

    private async Task<Guid> GetOrCreateCartIdAsync(string userId)
    {
        var id = await _context.Database.SqlQueryRaw<Guid>(
            "SELECT \"Id\" AS \"Value\" FROM \"Orders\" WHERE \"UserId\" = {0} AND \"Status\" = 'Pending'", userId
        ).FirstOrDefaultAsync();
        if (id != Guid.Empty) return id;

        id = Guid.NewGuid();
        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Orders\" (\"Id\", \"UserId\", \"Status\", \"Subtotal\", \"Tax\", \"Total\", \"ShippingAddress\", \"Notes\", \"CreatedAt\") VALUES ({id}, {userId}, 'Pending', 0, 0, 0, '', '', NOW() AT TIME ZONE 'UTC')");
        return id;
    }

    private async Task RecalculateOrderTotalsAsync(Guid orderId)
    {
        var sum = await _context.Database.SqlQueryRaw<decimal>(
            "SELECT COALESCE(SUM(\"UnitPrice\" * \"Quantity\"), 0) AS \"Value\" FROM \"OrderDetails\" WHERE \"OrderId\" = {0}", orderId
        ).FirstAsync();
        var tax = Math.Round(sum * 0.12m, 2);
        var total = Math.Round(sum + tax, 2);
        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE \"Orders\" SET \"Subtotal\" = {sum}, \"Tax\" = {tax}, \"Total\" = {total} WHERE \"Id\" = {orderId}");
    }

    private static string GenerateCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = Random.Shared;
        return $"BG-{new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray())}";
    }

    private record DetailRow(Guid Id, Guid OrderId, Guid? CourseId, Guid? PhysicalProductId, int Quantity, decimal UnitPrice);

    // ── Raw SQL helpers (bypass RowVersion concurrency) ─────

    private async Task<int> GetAvailableStockAsync(Guid productId) =>
        await _context.Database.SqlQueryRaw<int>(
            "SELECT COALESCE(\"Stock\", 0) - COALESCE(\"ReservedStock\", 0) AS \"Value\" FROM \"PhysicalProducts\" WHERE \"Id\" = {0}", productId
        ).FirstOrDefaultAsync();

    private async Task<int> ReserveStockAsync(Guid productId, int qty) =>
        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE \"PhysicalProducts\" SET \"ReservedStock\" = \"ReservedStock\" + {qty}, \"UpdatedAt\" = NOW() AT TIME ZONE 'UTC' WHERE \"Id\" = {productId} AND \"Stock\" - \"ReservedStock\" >= {qty}");

    private Task ReleaseStockAsync(Guid productId, int qty) =>
        _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE \"PhysicalProducts\" SET \"ReservedStock\" = GREATEST(0, \"ReservedStock\" - {qty}), \"UpdatedAt\" = NOW() AT TIME ZONE 'UTC' WHERE \"Id\" = {productId}");

    private async Task<int> GetAvailableSeatsAsync(Guid courseId) =>
        await _context.Database.SqlQueryRaw<int>(
            "SELECT COALESCE(\"TotalSeats\", 0) - COALESCE(\"ReservedSeats\", 0) - COALESCE(\"ConfirmedSeats\", 0) AS \"Value\" FROM \"Courses\" WHERE \"Id\" = {0}", courseId
        ).FirstOrDefaultAsync();

    private async Task<int> ReserveSeatAsync(Guid courseId, int qty) =>
        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE \"Courses\" SET \"ReservedSeats\" = \"ReservedSeats\" + {qty} WHERE \"Id\" = {courseId} AND \"TotalSeats\" - \"ReservedSeats\" - \"ConfirmedSeats\" >= {qty}");

    private Task ReleaseSeatAsync(Guid courseId, int qty) =>
        _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE \"Courses\" SET \"ReservedSeats\" = GREATEST(0, \"ReservedSeats\" - {qty}) WHERE \"Id\" = {courseId}");

    // ── Courses ─────────────────────────────────────────

    [HttpGet]
    public IActionResult CreateFromCourse() => RedirectToAction("Index", "Courses");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFromCourse(Guid courseId, int quantity, string paymentProvider)
    {
        var userId = await GetUserIdAsync();

        var available = await GetAvailableSeatsAsync(courseId);
        if (available < quantity)
            return Json(new { success = false, error = "No hay suficientes cupos disponibles." });

        var exists = await _context.Enrollments.AnyAsync(e => e.CourseId == courseId && e.UserId == userId && e.Status != "Cancelled");
        if (exists)
            return Json(new { success = false, error = "Ya estás inscrito en este curso." });

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var cartId = await GetOrCreateCartIdAsync(userId);
            var price = await _context.Courses.Where(c => c.Id == courseId).Select(c => c.Price).FirstAsync();

            var existingDetailId = await _context.Database.SqlQueryRaw<Guid?>(
                "SELECT \"Id\" AS \"Value\" FROM \"OrderDetails\" WHERE \"OrderId\" = {0} AND \"CourseId\" = {1}", cartId, courseId
            ).FirstOrDefaultAsync();

            if (existingDetailId.HasValue)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE \"OrderDetails\" SET \"Quantity\" = \"Quantity\" + {quantity}, \"UnitPrice\" = {price} WHERE \"Id\" = {existingDetailId.Value}");
            }
            else
            {
                var detailId = Guid.NewGuid();
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO \"OrderDetails\" (\"Id\", \"OrderId\", \"CourseId\", \"Quantity\", \"UnitPrice\") VALUES ({detailId}, {cartId}, {courseId}, {quantity}, {price})");
            }

            var affected = await ReserveSeatAsync(courseId, quantity);
            if (affected == 0)
            {
                await tx.RollbackAsync();
                return Json(new { success = false, error = "No hay suficientes cupos disponibles." });
            }

            var enrollment = new Enrollment
            {
                CourseId = courseId, UserId = userId, Status = "PendingPayment",
                OrderId = cartId, ConfirmationCode = GenerateCode()
            };
            _context.Enrollments.Add(enrollment);
            _context.InventoryMovements.Add(new InventoryMovement
            {
                CourseId = courseId, TipoMovimiento = "Reserva", Cantidad = quantity,
                Referencia = $"Enrollment:{enrollment.Id}", UsuarioId = userId
            });

            await RecalculateOrderTotalsAsync(cartId);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return Json(new { success = true, orderId = cartId, confirmationCode = enrollment.ConfirmationCode });
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return Json(new { success = false, error = ex.InnerException?.Message ?? ex.Message });
        }
    }

    // ── Products ────────────────────────────────────────

    [HttpGet]
    public IActionResult CreateFromProduct() => RedirectToAction("Index", "Products");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFromProduct(Guid productId, int quantity)
    {
        var userId = await GetUserIdAsync();

        var available = await GetAvailableStockAsync(productId);
        if (available < quantity)
        {
            TempData["Error"] = "Stock insuficiente.";
            return RedirectToAction("Index", "Products");
        }

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var cartId = await GetOrCreateCartIdAsync(userId);
            var price = await _context.PhysicalProducts.Where(p => p.Id == productId).Select(p => p.Price).FirstAsync();

            var existingDetailId = await _context.Database.SqlQueryRaw<Guid?>(
                "SELECT \"Id\" AS \"Value\" FROM \"OrderDetails\" WHERE \"OrderId\" = {0} AND \"PhysicalProductId\" = {1}", cartId, productId
            ).FirstOrDefaultAsync();

            if (existingDetailId.HasValue)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE \"OrderDetails\" SET \"Quantity\" = \"Quantity\" + {quantity}, \"UnitPrice\" = {price} WHERE \"Id\" = {existingDetailId.Value}");
            }
            else
            {
                var detailId = Guid.NewGuid();
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO \"OrderDetails\" (\"Id\", \"OrderId\", \"PhysicalProductId\", \"Quantity\", \"UnitPrice\") VALUES ({detailId}, {cartId}, {productId}, {quantity}, {price})");
            }

            var affected = await ReserveStockAsync(productId, quantity);
            if (affected == 0)
            {
                await tx.RollbackAsync();
                TempData["Error"] = "Stock insuficiente.";
                return RedirectToAction("Index", "Products");
            }

            _context.InventoryMovements.Add(new InventoryMovement
            {
                PhysicalProductId = productId, TipoMovimiento = "Reserva", Cantidad = quantity,
                Referencia = $"Cart:{cartId}:{productId}", UsuarioId = userId
            });

            await RecalculateOrderTotalsAsync(cartId);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["Success"] = "Producto agregado al carrito.";
            return RedirectToAction(nameof(Cart));
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await tx.RollbackAsync();
            var info = string.Join(" | ", ex.Entries.Select(e =>
            {
                var key = string.Join(",", e.Metadata.FindPrimaryKey()!.Properties.Select(p => $"{p.Name}={e.CurrentValues[p]}"));
                var mods = string.Join(",", e.Properties.Where(p => p.IsModified).Select(p => $"{p.Metadata.Name}={p.OriginalValue}->{p.CurrentValue}"));
                return $"{e.Entity.GetType().Name} state={e.State} key=[{key}] modified=[{mods}]";
            }));
            TempData["Error"] = $"Conflicto: {info}";
            return RedirectToAction("Index", "Products");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            TempData["Error"] = $"Error: {ex.InnerException?.Message ?? ex.Message}";
            return RedirectToAction("Index", "Products");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(Guid detailId, int quantity)
    {
        var userId = await GetUserIdAsync();

        var detail = await _context.Database.SqlQueryRaw<DetailRow>(
            "SELECT od.\"Id\", od.\"OrderId\", od.\"CourseId\", od.\"PhysicalProductId\", od.\"Quantity\", od.\"UnitPrice\" FROM \"OrderDetails\" od INNER JOIN \"Orders\" o ON o.\"Id\" = od.\"OrderId\" WHERE od.\"Id\" = {0} AND o.\"UserId\" = {1} AND o.\"Status\" = 'Pending'",
            detailId, userId
        ).FirstOrDefaultAsync();
        if (detail == null) return NotFound();
        if (quantity < 1) return RedirectToAction(nameof(Cart));

        var diff = quantity - detail.Quantity;
        if (diff == 0) return RedirectToAction(nameof(Cart));

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            if (detail.CourseId != null)
            {
                if (diff > 0)
                {
                    var affected = await ReserveSeatAsync(detail.CourseId.Value, diff);
                    if (affected == 0)
                    {
                        await tx.RollbackAsync();
                        TempData["Error"] = "No hay suficientes cupos disponibles.";
                        return RedirectToAction(nameof(Cart));
                    }
                }
                else
                {
                    await ReleaseSeatAsync(detail.CourseId.Value, -diff);
                }
            }

            if (detail.PhysicalProductId != null)
            {
                if (diff > 0)
                {
                    var affected = await ReserveStockAsync(detail.PhysicalProductId.Value, diff);
                    if (affected == 0)
                    {
                        await tx.RollbackAsync();
                        TempData["Error"] = "Stock insuficiente.";
                        return RedirectToAction(nameof(Cart));
                    }
                }
                else
                {
                    await ReleaseStockAsync(detail.PhysicalProductId.Value, -diff);
                }
            }

            var price = detail.UnitPrice;
            if (detail.CourseId != null)
                price = await _context.Courses.Where(c => c.Id == detail.CourseId).Select(c => c.Price).FirstAsync();
            else if (detail.PhysicalProductId != null)
                price = await _context.PhysicalProducts.Where(p => p.Id == detail.PhysicalProductId).Select(p => p.Price).FirstAsync();

            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE \"OrderDetails\" SET \"Quantity\" = {quantity}, \"UnitPrice\" = {price} WHERE \"Id\" = {detailId}");

            await RecalculateOrderTotalsAsync(detail.OrderId);
            await tx.CommitAsync();

            TempData["Success"] = "Cantidad actualizada.";
            return RedirectToAction(nameof(Cart));
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            TempData["Error"] = $"Error: {ex.InnerException?.Message ?? ex.Message}";
            return RedirectToAction(nameof(Cart));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDetail(Guid detailId)
    {
        var userId = await GetUserIdAsync();

        var detail = await _context.Database.SqlQueryRaw<DetailRow>(
            "SELECT od.\"Id\", od.\"OrderId\", od.\"CourseId\", od.\"PhysicalProductId\", od.\"Quantity\" AS \"Quantity\", od.\"UnitPrice\" FROM \"OrderDetails\" od INNER JOIN \"Orders\" o ON o.\"Id\" = od.\"OrderId\" WHERE od.\"Id\" = {0} AND o.\"UserId\" = {1} AND o.\"Status\" = 'Pending'",
            detailId, userId
        ).FirstOrDefaultAsync();
        if (detail == null) return NotFound();

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            if (detail.CourseId != null)
                await ReleaseSeatAsync(detail.CourseId.Value, detail.Quantity);

            if (detail.PhysicalProductId != null)
                await ReleaseStockAsync(detail.PhysicalProductId.Value, detail.Quantity);

            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM \"OrderDetails\" WHERE \"Id\" = {detailId}");

            var hasItems = await _context.Database.SqlQueryRaw<int>(
                "SELECT COUNT(*) AS \"Value\" FROM \"OrderDetails\" WHERE \"OrderId\" = {0}", detail.OrderId
            ).FirstAsync();

            if (hasItems == 0)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM \"Orders\" WHERE \"Id\" = {detail.OrderId}");
            }
            else
            {
                await RecalculateOrderTotalsAsync(detail.OrderId);
            }

            await tx.CommitAsync();

            TempData["Success"] = "Artículo eliminado del carrito.";
            return RedirectToAction(nameof(Cart));
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            TempData["Error"] = $"Error: {ex.InnerException?.Message ?? ex.Message}";
            return RedirectToAction(nameof(Cart));
        }
    }

    // ── Checkout / Cart / Orders ────────────────────────

    public async Task<IActionResult> Checkout(Guid? orderId)
    {
        var userId = await GetUserIdAsync();
        var order = await _context.Orders
            .Include(o => o.Details).ThenInclude(d => d.Course)
            .Include(o => o.Details).ThenInclude(d => d.PhysicalProduct)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Pending"
                && (orderId == null || o.Id == orderId));
        if (order == null || !order.Details.Any())
        {
            TempData["Error"] = "No hay artículos en el carrito.";
            return RedirectToAction(nameof(Cart));
        }
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(Guid orderId, string paymentProvider,
        string? Email, string? Phone, string? ShippingAddress, string? City, string? Province, string? PostalCode)
    {
        var userId = await GetUserIdAsync();
        var order = await _context.Orders
            .Include(o => o.Details).ThenInclude(d => d.Course)
            .Include(o => o.Details).ThenInclude(d => d.PhysicalProduct)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId && o.Status == "Pending");
        if (order == null) return NotFound();

        order.ShippingAddress = ShippingAddress ?? "";

        var clientTxId = Guid.NewGuid().ToString("N");
        var payment = new Payment
        {
            OrderId = order.Id, Provider = paymentProvider, Amount = order.Total,
            Currency = "USD", Status = "Pending", ExternalId = clientTxId, GatewayResponse = "{}"
        };
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        string redirectUrl;
        if (paymentProvider == "PayPal")
        {
            var result = await _payPal.CreateOrderAsync(order.Total, $"Order:{order.Id}");
            payment.ExternalId = result.OrderId;
            payment.GatewayResponse = result.RawResponse;
            redirectUrl = result.ApprovalUrl;
        }
        else
        {
            var link = await _payPhone.CreatePaymentLinkAsync(order.Total, clientTxId, $"Order:{order.Id}");
            payment.ExternalId = clientTxId;
            redirectUrl = link;
        }
        await _context.SaveChangesAsync();
        return Redirect(redirectUrl);
    }

    public async Task<IActionResult> Cart()
    {
        var userId = await GetUserIdAsync();
        var order = await _context.Orders.AsNoTracking()
            .Include(o => o.Details).ThenInclude(d => d.Course)
            .Include(o => o.Details).ThenInclude(d => d.PhysicalProduct)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Pending");
        return View(order);
    }

    public async Task<IActionResult> MyOrders()
    {
        var userId = await GetUserIdAsync();
        var orders = await _context.Orders
            .Include(o => o.Details).ThenInclude(d => d.Course)
            .Include(o => o.Payment)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> Confirmation(Guid orderId)
    {
        var userId = await GetUserIdAsync();
        var order = await _context.Orders
            .Include(o => o.Details).ThenInclude(d => d.Course)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        if (order == null) return NotFound();

        if (order.Payment?.Status == "Approved")
        {
            var email = User.Identity!.Name ?? "";
            await _email.SendConfirmationAsync(email, "Pedido confirmado - BioGama Ecuador",
                $"Tu pedido #{order.Id} fue confirmado. Total: ${order.Total}");
        }
        return View(order);
    }
}