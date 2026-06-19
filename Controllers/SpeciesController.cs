using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models;

namespace BioGamaEcuador.Controllers
{
    [Authorize]
    public class SpeciesController : Controller
    {
        private readonly AppDbContext _context;

        public SpeciesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Species — los 3 roles
        [Authorize(Roles = "Administrador,Investigador,UsuarioPublico")]
        public async Task<IActionResult> Index(string busqueda, string familia, string estado, string endemica, int pagina = 1)
        {
            const int tamano = 50;
            var query = _context.Species.AsNoTracking().Include(s => s.Family).Where(s => s.IsActive);
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(s => EF.Functions.ILike(s.CommonName, $"%{busqueda}%") || EF.Functions.Like(s.ScientificName, $"%{busqueda}%"));
            }
            if (!string.IsNullOrWhiteSpace(familia))
            {
                query = query.Where(s => s.Family != null && s.Family.Name == familia);
            }
            if (!string.IsNullOrWhiteSpace(estado))
            {
                query = query.Where(s => s.ConservationStatus == estado);
            }
            if (!string.IsNullOrWhiteSpace(endemica))
            {
                if (bool.TryParse(endemica, out var isEndemic))
                {
                    query = query.Where(s => s.IsEndemic == isEndemic);
                }
            }

            int total = await query.CountAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling(total / (double)tamano);
            ViewBag.TotalItems = total;

            var datos = await query
                .OrderBy(s => s.Id)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();

            // populate filter lists for the view
            ViewBag.Busqueda = busqueda;
            ViewBag.Familias = await _context.Families.Where(f => f.IsActive).Select(f => f.Name).Distinct().OrderBy(n => n).ToListAsync();
            ViewBag.Estados = await _context.Species.Select(s => s.ConservationStatus).Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s).ToListAsync();
            ViewBag.FamiliaFiltro = familia;
            ViewBag.EstadoFiltro = estado;
            ViewBag.EndémicaFiltro = string.IsNullOrWhiteSpace(endemica) ? (bool?)null : (endemica == "true");
            return View(datos);
        }

        // GET: Species/Details/5 — los 3 roles
        [Authorize(Roles = "Administrador,Investigador,UsuarioPublico")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var species = await _context.Species
                .Include(s => s.Family)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (species == null) return NotFound();

            return View(species);
        }

        // GET: Species/Create — Admin e Investigador
        [Authorize(Roles = "Administrador,Investigador")]
        public IActionResult Create()
        {
            ViewData["FamilyId"] = new SelectList(_context.Families, "Id", "Name");
            return View();
        }

        // POST: Species/Create — Admin e Investigador
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Create([Bind("Id,CommonName,ScientificName,ConservationStatus,Description,ImageUrl,IsEndemic,FamilyId,IsActive,CreatedAt")] Species species)
        {
            if (species.FamilyId <= 0)
            {
                ModelState.AddModelError("FamilyId", "Debe seleccionar una familia.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    species.IsActive = true;
                    species.CreatedAt = DateTime.UtcNow;
                    _context.Add(species);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                }
            }
            ViewData["FamilyId"] = new SelectList(_context.Families, "Id", "Name", species.FamilyId);
            return View(species);
        }

        // GET: Species/Edit/5 — Admin e Investigador
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var species = await _context.Species.FindAsync(id);
            if (species == null) return NotFound();

            ViewData["FamilyId"] = new SelectList(_context.Families, "Id", "Kingdom", species.FamilyId);
            return View(species);
        }

        // POST: Species/Edit/5 — Admin e Investigador
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CommonName,ScientificName,ConservationStatus,Description,ImageUrl,IsEndemic,FamilyId,IsActive,CreatedAt")] Species species)
        {
            if (id != species.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure CreatedAt is UTC before saving (HTML posts often produce Unspecified kind)
                    var created = species.CreatedAt;
                    if (created.Kind == DateTimeKind.Local)
                    {
                        created = created.ToUniversalTime();
                    }
                    else if (created.Kind == DateTimeKind.Unspecified)
                    {
                        created = DateTime.SpecifyKind(created, DateTimeKind.Utc);
                    }
                    species.CreatedAt = created;

                    _context.Update(species);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpeciesExists(species.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FamilyId"] = new SelectList(_context.Families, "Id", "Kingdom", species.FamilyId);
            return View(species);
        }

        // GET: Species/Delete/5 — Solo Administrador
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var species = await _context.Species
                .Include(s => s.Family)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (species == null) return NotFound();

            return View(species);
        }

        // POST: Species/Delete/5 — Solo Administrador
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var species = await _context.Species.FindAsync(id);
            if (species != null)
            {
                species.IsActive = false;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpeciesExists(int id)
        {
            return _context.Species.Any(e => e.Id == id);
        }
    }
}