using BioGamaEcuador.Data;
using BioGamaEcuador.Models.Admin;
using BioGamaEcuador.Models.Sales;
using BioGamaEcuador.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BioGamaEcuador.Controllers;

[Authorize(Roles = "Admin,Administrador")]
[Route("Admin/Products")]
public sealed class AdminProductsController(AppDbContext context, IAuditService audit) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? search, string? sku, bool lowStock = false)
    { var q=context.PhysicalProducts.AsNoTracking().AsQueryable(); if (!string.IsNullOrWhiteSpace(search)) q=q.Where(p=>EF.Functions.ILike(p.Name,$"%{search.Trim()}%")); if (!string.IsNullOrWhiteSpace(sku)) q=q.Where(p=>EF.Functions.ILike(p.SKU,$"%{sku.Trim()}%")); if(lowStock) q=q.Where(p=>p.Stock-p.ReservedStock<=p.MinStock); ViewBag.Search=search;ViewBag.Sku=sku;ViewBag.LowStock=lowStock;return View(await q.OrderBy(p=>p.Name).ToListAsync()); }
    [HttpGet("Create")] public async Task<IActionResult> Create() { await SpeciesAsync(); return View(new ProductFormViewModel()); }
    [HttpPost("Create"), ValidateAntiForgeryToken] public async Task<IActionResult> Create(ProductFormViewModel model) { await SkuAsync(model.SKU); if(!ModelState.IsValid){await SpeciesAsync();return View(model);}var p=new PhysicalProduct();Apply(model,p);context.PhysicalProducts.Add(p);await context.SaveChangesAsync();TempData["Success"]="Producto creado.";return RedirectToAction(nameof(Index)); }
    [HttpGet("Edit/{id:guid}")] public async Task<IActionResult> Edit(Guid id){var p=await context.PhysicalProducts.FindAsync(id);if(p is null)return NotFound();await SpeciesAsync();return View(ToForm(p));}
    [HttpPost("Edit/{id:guid}"), ValidateAntiForgeryToken] public async Task<IActionResult> Edit(Guid id, ProductFormViewModel model){if(id!=model.Id)return BadRequest();var p=await context.PhysicalProducts.SingleOrDefaultAsync(x=>x.Id==id);if(p is null)return NotFound();await SkuAsync(model.SKU,id);if(model.Stock<p.ReservedStock)ModelState.AddModelError(nameof(model.Stock),"No puede ser menor al stock reservado.");if(!ModelState.IsValid){await SpeciesAsync();return View(model);}Apply(model,p);await context.SaveChangesAsync();TempData["Success"]="Producto actualizado.";return RedirectToAction(nameof(Index));}
    [HttpPost("Delete/{id:guid}"), ValidateAntiForgeryToken] public async Task<IActionResult> Delete(Guid id){var p=await context.PhysicalProducts.SingleOrDefaultAsync(x=>x.Id==id);if(p is null)return NotFound();p.DeletedAt=DateTime.UtcNow;p.UpdatedAt=DateTime.UtcNow;await audit.LogAsync("SoftDelete", "PhysicalProduct", p.Id.ToString(), null, null, "system", null);await context.SaveChangesAsync();TempData["Success"]="Producto eliminado.";return RedirectToAction(nameof(Index));}
    [HttpGet("Delete/{id:guid}")] public async Task<IActionResult> DeleteConfirmation(Guid id){var product=await context.PhysicalProducts.AsNoTracking().SingleOrDefaultAsync(p=>p.Id==id);return product is null?NotFound():View("Delete",product);}
    private async Task SkuAsync(string sku, Guid? id=null){if(await context.PhysicalProducts.AnyAsync(p=>p.SKU==sku.Trim()&&p.Id!=id))ModelState.AddModelError(nameof(ProductFormViewModel.SKU),"El SKU ya está registrado.");}
    private async Task SpeciesAsync()=>ViewBag.SpeciesId=new SelectList(await context.Species.OrderBy(s=>s.CommonName).ToListAsync(),"Id","CommonName");
    private static void Apply(ProductFormViewModel m,PhysicalProduct p){p.Name=m.Name.Trim();p.Description=m.Description;p.Price=m.Price;p.Stock=m.Stock;p.MinStock=m.MinStock;p.SKU=m.SKU.Trim();p.ImageUrl=m.ImageUrl;p.IsActive=m.IsActive;p.SpeciesId=m.SpeciesId;p.UpdatedAt=DateTime.UtcNow;}
    private static ProductFormViewModel ToForm(PhysicalProduct p)=>new(){Id=p.Id,Name=p.Name,Description=p.Description,Price=p.Price,Stock=p.Stock,MinStock=p.MinStock,SKU=p.SKU,ImageUrl=p.ImageUrl,IsActive=p.IsActive,SpeciesId=p.SpeciesId};
}
