using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models;

namespace BioGamaEcuador.Controllers
{
    [Authorize]
    public class FamiliesController : Controller
    {
        private readonly AppDbContext _context;

        public FamiliesController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrador,Investigador,UsuarioPublico")]
        public async Task<IActionResult> Index(string busqueda, string reino, int pagina = 1)
        {
            const int tamano = 50;

            var query = _context.Families.AsNoTracking().Where(f => f.IsActive);
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(f => EF.Functions.ILike(f.Name, $"%{busqueda}%") || EF.Functions.ILike(f.Kingdom, $"%{busqueda}%"));
            }
            if (!string.IsNullOrWhiteSpace(reino))
            {
                query = query.Where(f => f.Kingdom == reino);
            }

            int total = await query.CountAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling(total / (double)tamano);
            ViewBag.TotalItems = total;

            var datos = await query
                .OrderBy(f => f.Id)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();

            ViewBag.Busqueda = busqueda;
            ViewBag.Kingdoms = await _context.Families.Select(f => f.Kingdom).Where(k => !string.IsNullOrEmpty(k)).Distinct().OrderBy(k => k).ToListAsync();
            ViewBag.ReinoFiltro = reino;

            return View(datos);
        }

        [Authorize(Roles = "Administrador,Investigador,UsuarioPublico")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var family = await _context.Families
                .FirstOrDefaultAsync(m => m.Id == id);
            if (family == null) return NotFound();

            return View(family);
        }

        [Authorize(Roles = "Administrador,Investigador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Create([Bind("Name,Kingdom")] Family family)
        {
            if (ModelState.IsValid)
            {
                family.IsActive = true;
                family.CreatedAt = DateTime.UtcNow;
                _context.Add(family);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(family);
        }

        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var family = await _context.Families.FindAsync(id);
            if (family == null) return NotFound();
            return View(family);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Kingdom,IsActive,CreatedAt")] Family family)
        {
            if (id != family.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var created = family.CreatedAt;
                    if (created.Kind == DateTimeKind.Local)
                    {
                        created = created.ToUniversalTime();
                    }
                    else if (created.Kind == DateTimeKind.Unspecified)
                    {
                        created = DateTime.SpecifyKind(created, DateTimeKind.Utc);
                    }
                    family.CreatedAt = created;

                    _context.Update(family);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Families.Any(e => e.Id == family.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(family);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var family = await _context.Families
                .FirstOrDefaultAsync(m => m.Id == id);
            if (family == null) return NotFound();

            return View(family);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var family = await _context.Families.FindAsync(id);
            if (family != null)
            {
                family.IsActive = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
