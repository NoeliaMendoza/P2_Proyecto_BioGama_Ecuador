using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;

namespace BioGamaEcuador.Controllers;

public class ProductsController(AppDbContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        const int pageSize = 12;
        var q = context.PhysicalProducts.AsNoTracking().Where(p => p.IsActive && p.DeletedAt == null);
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(p => EF.Functions.ILike(p.Name, $"%{search.Trim()}%"));
        var total = await q.CountAsync();
        ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        ViewBag.Page = Math.Clamp(page, 1, (int)ViewBag.TotalPages);
        ViewBag.Search = search;
        ViewBag.TotalProducts = total;
        ViewBag.AvailableUnits = await q.SumAsync(p => (int?)(p.Stock - p.ReservedStock)) ?? 0;
        return View(await q.OrderBy(p => p.Name).Skip(((int)ViewBag.Page - 1) * pageSize).Take(pageSize).ToListAsync());
    }
}
