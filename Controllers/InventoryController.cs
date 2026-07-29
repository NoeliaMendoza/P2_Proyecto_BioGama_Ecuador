using System.Security.Claims;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models.Admin;
using BioGamaEcuador.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BioGamaEcuador.Controllers;

[Authorize(Roles = "Admin,Administrador")]
[Route("Admin/Inventory")]
public sealed class InventoryController(AppDbContext context, IInventoryService inventory, IEmailService email, IAuditService audit) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? type, Guid? productId, Guid? courseId, DateTime? from, DateTime? to)
    {
        var q = context.InventoryMovements
            .Include(m => m.PhysicalProduct)
            .Include(m => m.Course)
            .AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(type)) q = q.Where(m => m.TipoMovimiento == type);
        if (productId.HasValue) q = q.Where(m => m.PhysicalProductId == productId);
        if (courseId.HasValue) q = q.Where(m => m.CourseId == courseId);
        if (from.HasValue) q = q.Where(m => m.FechaMovimiento >= from.Value.Date);
        if (to.HasValue) q = q.Where(m => m.FechaMovimiento < to.Value.Date.AddDays(1));
        ViewBag.Types = new SelectList(new[] { "Entrada", "Salida", "Ajuste", "Reserva", "Confirmacion", "Liberacion", "Compra", "Devolucion", "Transferencia" }, type);
        ViewBag.Products = new SelectList(await context.PhysicalProducts.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", productId);
        ViewBag.Courses = new SelectList(await context.Courses.OrderBy(c => c.Title).ToListAsync(), "Id", "Title", courseId);
        ViewBag.From = from?.ToString("yyyy-MM-dd"); ViewBag.To = to?.ToString("yyyy-MM-dd");
        return View(await q.OrderByDescending(m => m.FechaMovimiento).Take(300).ToListAsync());
    }

    [HttpPost("Entry"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Entry(InventoryTransactionViewModel m)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Revisa los datos del movimiento."; return RedirectToAction(nameof(Index)); }
        try { await inventory.EntryAsync(m.ProductId, m.Quantity, m.Reference, UserId(), m.Notes); await audit.LogAsync("InventoryEntry", "PhysicalProduct", m.ProductId.ToString(), null, $"+{m.Quantity}", UserId(), HttpContext.Connection.RemoteIpAddress?.ToString()); TempData["Success"] = "Entrada registrada."; }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
    [HttpPost("Exit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Exit(InventoryTransactionViewModel m)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Revisa los datos del movimiento."; return RedirectToAction(nameof(Index)); }
        try
        {
            await inventory.ExitAsync(m.ProductId, m.Quantity, m.Reference, UserId(), m.Notes);
            await audit.LogAsync("InventoryExit", "PhysicalProduct", m.ProductId.ToString(), null, $"-{m.Quantity}", UserId(), HttpContext.Connection.RemoteIpAddress?.ToString());
            await CheckLowStockAsync(m.ProductId);
            TempData["Success"] = "Salida registrada.";
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
    [HttpPost("Adjustment"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Adjustment(InventoryAdjustmentViewModel m)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Revisa los datos del ajuste."; return RedirectToAction(nameof(Index)); }
        try { await inventory.AdjustAsync(m.ProductId, m.NewStock, m.Reference, UserId(), m.Notes); await audit.LogAsync("InventoryAdjustment", "PhysicalProduct", m.ProductId.ToString(), null, $"NewStock={m.NewStock}", UserId(), HttpContext.Connection.RemoteIpAddress?.ToString()); await CheckLowStockAsync(m.ProductId); TempData["Success"] = "Ajuste registrado."; }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
    private async Task<IActionResult> Operate(InventoryTransactionViewModel m, Func<InventoryTransactionViewModel, string, Task> action, string ok)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Revisa los datos del movimiento."; return RedirectToAction(nameof(Index)); }
        try { await action(m, UserId()); TempData["Success"] = ok; }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
    private string UserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";

    private async Task CheckLowStockAsync(Guid productId)
    {
        var product = await context.PhysicalProducts.FindAsync(productId);
        if (product != null && product.Stock - product.ReservedStock <= product.MinStock)
        {
            var admins = await context.Users
                .Where(u => context.UserRoles.Any(ur => ur.UserId == u.Id && context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Admin")))
                .ToListAsync();
            foreach (var admin in admins)
            {
                if (admin.Email != null)
                    await email.SendLowStockAlertAsync(admin.Email, product.Name, product.Stock - product.ReservedStock);
            }
        }
    }
}
