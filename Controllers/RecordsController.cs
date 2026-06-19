using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models;

namespace BioGamaEcuador.Controllers
{
    [Authorize]
    public class RecordsController : Controller
    {
        private readonly AppDbContext _context;

        public RecordsController(AppDbContext context)
        {
            _context = context;
        }

        private async Task<Researcher?> GetCurrentResearcherAsync()
        {
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userEmail)) return null;
            return await _context.Researchers.FirstOrDefaultAsync(r => r.Email == userEmail);
        }

        [Authorize(Roles = "Administrador,Investigador,UsuarioPublico")]
        public async Task<IActionResult> Index(string busqueda, string speciesId, string locationId, string researcherId, int pagina = 1)
        {
            const int tamano = 50;
            var query = _context.Records.AsNoTracking()
                .Include(r => r.Species)
                .Include(r => r.Location)
                .Include(r => r.Researcher)
                .Where(r => r.IsActive);

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(r => (r.Species != null && EF.Functions.ILike(r.Species.CommonName, $"%{busqueda}%"))
                                         || (r.Location != null && EF.Functions.ILike(r.Location.PlaceName, $"%{busqueda}%"))
                                         || (r.Researcher != null && EF.Functions.ILike(r.Researcher.Name, $"%{busqueda}%")));
            }
            if (!string.IsNullOrWhiteSpace(speciesId) && int.TryParse(speciesId, out var sid))
            {
                query = query.Where(r => r.SpeciesId == sid);
            }
            if (!string.IsNullOrWhiteSpace(locationId) && int.TryParse(locationId, out var lid))
            {
                query = query.Where(r => r.LocationId == lid);
            }
            if (!string.IsNullOrWhiteSpace(researcherId) && int.TryParse(researcherId, out var rid))
            {
                query = query.Where(r => r.ResearcherId == rid);
            }

            int total = await query.CountAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling(total / (double)tamano);
            ViewBag.TotalItems = total;

            var datos = await query
                .OrderBy(r => r.Id)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();

            ViewBag.Busqueda = busqueda;
            var speciesList = await _context.Species.Where(s => s.IsActive).OrderBy(s => s.CommonName).ToListAsync();
            var locationList = await _context.Locations.Where(l => l.IsActive).OrderBy(l => l.PlaceName).ToListAsync();
            var researcherList = await _context.Researchers.Where(r => r.IsActive).OrderBy(r => r.Name).ToListAsync();
            ViewBag.SpeciesList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(speciesList, "Id", "CommonName");
            ViewBag.LocationList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(locationList, "Id", "PlaceName");
            ViewBag.ResearcherList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(researcherList, "Id", "Name");
            ViewBag.SpeciesFiltro = speciesId;
            ViewBag.LocationFiltro = locationId;
            ViewBag.ResearcherFiltro = researcherId;
            return View(datos);
        }

        [Authorize(Roles = "Administrador,Investigador,UsuarioPublico")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.Records
                .Include(r => r.Species)
                .Include(r => r.Location)
                .Include(r => r.Researcher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (entry == null) return NotFound();
            return View(entry);
        }

        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Create()
        {
            var currentResearcher = await GetCurrentResearcherAsync();
            bool isInvestigator = User.IsInRole("Investigador") && !User.IsInRole("Administrador");
            if (isInvestigator && currentResearcher == null)
            {
                return Forbid();
            }

            ViewData["SpeciesId"] = new SelectList(_context.Species, "Id", "CommonName");
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "PlaceName");
            ViewData["IsCurrentInvestigator"] = isInvestigator;
            ViewData["CurrentResearcherId"] = currentResearcher?.Id;
            ViewData["CurrentResearcherName"] = currentResearcher?.Name;
            ViewData["ResearcherId"] = isInvestigator && currentResearcher != null
                ? new SelectList(new[] { currentResearcher }, "Id", "Name", currentResearcher.Id)
                : new SelectList(_context.Researchers, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Create(Record entry)
        {
            var isInvestigator = User.IsInRole("Investigador") && !User.IsInRole("Administrador");
            Researcher? currentResearcher = null;
            if (isInvestigator)
            {
                currentResearcher = await GetCurrentResearcherAsync();
                if (currentResearcher == null) return Forbid();
                entry.ResearcherId = currentResearcher.Id;
            }

            if (ModelState.IsValid)
            {
                entry.IsActive = true;
                entry.CreatedAt = DateTime.UtcNow;
                _context.Add(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["SpeciesId"] = new SelectList(_context.Species, "Id", "CommonName", entry.SpeciesId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "PlaceName", entry.LocationId);
            ViewData["IsCurrentInvestigator"] = isInvestigator;
            ViewData["CurrentResearcherId"] = currentResearcher?.Id;
            ViewData["CurrentResearcherName"] = currentResearcher?.Name;
            ViewData["ResearcherId"] = isInvestigator && currentResearcher != null
                ? new SelectList(new[] { currentResearcher }, "Id", "Name", currentResearcher.Id)
                : new SelectList(_context.Researchers, "Id", "Name", entry.ResearcherId);
            return View(entry);
        }

        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.Records.FindAsync(id);
            if (entry == null) return NotFound();

            // If current user is Investigador (not Administrador), ensure they own the record
            var isInvestigator = User.IsInRole("Investigador") && !User.IsInRole("Administrador");
            Researcher? currentResearcher = null;
            if (isInvestigator)
            {
                currentResearcher = await GetCurrentResearcherAsync();
                if (currentResearcher == null || currentResearcher.Id != entry.ResearcherId)
                {
                    return Forbid();
                }
            }

            ViewData["SpeciesId"] = new SelectList(_context.Species, "Id", "CommonName", entry.SpeciesId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "PlaceName", entry.LocationId);
            ViewData["IsCurrentInvestigator"] = isInvestigator;
            ViewData["CurrentResearcherId"] = currentResearcher?.Id;
            ViewData["CurrentResearcherName"] = currentResearcher?.Name ?? entry.Researcher?.Name;
            ViewData["ResearcherId"] = isInvestigator && currentResearcher != null
                ? new SelectList(new[] { currentResearcher }, "Id", "Name", entry.ResearcherId)
                : new SelectList(_context.Researchers, "Id", "Name", entry.ResearcherId);
            return View(entry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Edit(int id, Record entry)
        {
            if (id != entry.Id) return NotFound();

            var isInvestigator = User.IsInRole("Investigador") && !User.IsInRole("Administrador");
            if (isInvestigator)
            {
                var currentResearcher = await GetCurrentResearcherAsync();
                if (currentResearcher == null) return Forbid();
                entry.ResearcherId = currentResearcher.Id;
            }

            if (ModelState.IsValid)
            {
                var created = entry.CreatedAt;
                if (created.Kind == DateTimeKind.Local)
                {
                    created = created.ToUniversalTime();
                }
                else if (created.Kind == DateTimeKind.Unspecified)
                {
                    created = DateTime.SpecifyKind(created, DateTimeKind.Utc);
                }
                entry.CreatedAt = created;

                _context.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpeciesId"] = new SelectList(_context.Species, "Id", "CommonName", entry.SpeciesId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "PlaceName", entry.LocationId);
            ViewData["ResearcherId"] = new SelectList(_context.Researchers, "Id", "Name", entry.ResearcherId);
            return View(entry);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.Records
                .Include(r => r.Species)
                .Include(r => r.Location)
                .Include(r => r.Researcher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (entry == null) return NotFound();
            return View(entry);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entry = await _context.Records.FindAsync(id);
            if (entry != null)
            {
                entry.IsActive = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}