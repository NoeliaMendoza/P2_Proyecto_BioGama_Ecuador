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
public sealed class InventoryController(AppDbContext context, IInventoryMovementService inventory) : Controller
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
    public Task<IActionResult> Entry(InventoryTransactionViewModel m) => Operate(m, (x, u) => inventory.EntryAsync(x.ProductId, x.Quantity, x.Reference, u, x.Notes), "Entrada registrada.");
    [HttpPost("Exit"), ValidateAntiForgeryToken]
    public Task<IActionResult> Exit(InventoryTransactionViewModel m) => Operate(m, (x, u) => inventory.ExitAsync(x.ProductId, x.Quantity, x.Reference, u, x.Notes), "Salida registrada.");
    [HttpPost("Adjustment"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Adjustment(InventoryAdjustmentViewModel m)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Revisa los datos del ajuste."; return RedirectToAction(nameof(Index)); }
        try { await inventory.AdjustAsync(m.ProductId, m.NewStock, m.Reference, UserId(), m.Notes); TempData["Success"] = "Ajuste registrado."; }
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
}
