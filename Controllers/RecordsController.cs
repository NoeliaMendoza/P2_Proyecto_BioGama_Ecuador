using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models;

namespace BioGamaEcuador.Controllers
{
    public class RecordsController : Controller
    {
        private readonly AppDbContext _context;

        public RecordsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var records = await _context.Records
                .Include(r => r.Species)
                .Include(r => r.Location)
                .Include(r => r.Researcher)
                .ToListAsync();
            return View(records);
        }

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

        public IActionResult Create()
        {
            ViewData["SpeciesId"] = new SelectList(_context.Species, "Id", "CommonName");
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "PlaceName");
            ViewData["ResearcherId"] = new SelectList(_context.Researchers, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Record entry)
        {
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
            ViewData["ResearcherId"] = new SelectList(_context.Researchers, "Id", "Name", entry.ResearcherId);
            return View(entry);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.Records.FindAsync(id);
            if (entry == null) return NotFound();

            ViewData["SpeciesId"] = new SelectList(_context.Species, "Id", "CommonName", entry.SpeciesId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "PlaceName", entry.LocationId);
            ViewData["ResearcherId"] = new SelectList(_context.Researchers, "Id", "Name", entry.ResearcherId);
            return View(entry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Record entry)
        {
            if (id != entry.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpeciesId"] = new SelectList(_context.Species, "Id", "CommonName", entry.SpeciesId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "PlaceName", entry.LocationId);
            ViewData["ResearcherId"] = new SelectList(_context.Researchers, "Id", "Name", entry.ResearcherId);
            return View(entry);
        }

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