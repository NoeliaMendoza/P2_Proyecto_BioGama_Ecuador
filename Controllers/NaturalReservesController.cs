using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models;

namespace BioGamaEcuador.Controllers
{
    [Authorize]
    public class NaturalReservesController : Controller
    {
        private readonly AppDbContext _context;

        public NaturalReservesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: NaturalReserves
        [Authorize(Roles = "Administrador,Investigador,UsuarioPublico")]
        public async Task<IActionResult> Index(string busqueda, string region, int pagina = 1)
        {
            const int tamano = 50;
            var query = _context.NaturalReserves.AsNoTracking().Where(n => n.IsActive);
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(n => EF.Functions.ILike(n.Name, $"%{busqueda}%") || EF.Functions.Like(n.Region, $"%{busqueda}%"));
            }
            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(n => n.Region == region);
            }

            int total = await query.CountAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling(total / (double)tamano);
            ViewBag.TotalItems = total;

            var datos = await query
                .OrderBy(n => n.Id)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();

            ViewBag.Busqueda = busqueda;
            ViewBag.Regiones = await _context.NaturalReserves.Select(n => n.Region).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToListAsync();
            ViewBag.RegionFiltro = region;
            return View(datos);
        }

        // GET: NaturalReserves/Details/5
        [Authorize(Roles = "Administrador,Investigador,UsuarioPublico")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var naturalReserve = await _context.NaturalReserves
                .FirstOrDefaultAsync(m => m.Id == id);
            if (naturalReserve == null)
            {
                return NotFound();
            }

            return View(naturalReserve);
        }

        // GET: NaturalReserves/Create
        [Authorize(Roles = "Administrador,Investigador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: NaturalReserves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Create([Bind("Id,Name,Region,SurfaceHectares,YearCreated,Description,IsActive,CreatedAt")] NaturalReserve naturalReserve)
        {
            if (ModelState.IsValid)
            {
                _context.Add(naturalReserve);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(naturalReserve);
        }

        // GET: NaturalReserves/Edit/5
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var naturalReserve = await _context.NaturalReserves.FindAsync(id);
            if (naturalReserve == null)
            {
                return NotFound();
            }
            return View(naturalReserve);
        }

        // POST: NaturalReserves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Region,SurfaceHectares,YearCreated,Description,IsActive,CreatedAt")] NaturalReserve naturalReserve)
        {
            if (id != naturalReserve.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var created = naturalReserve.CreatedAt;
                    if (created.Kind == DateTimeKind.Local)
                    {
                        created = created.ToUniversalTime();
                    }
                    else if (created.Kind == DateTimeKind.Unspecified)
                    {
                        created = DateTime.SpecifyKind(created, DateTimeKind.Utc);
                    }
                    naturalReserve.CreatedAt = created;

                    naturalReserve.UpdatedAt = DateTime.UtcNow;
                    _context.Update(naturalReserve);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NaturalReserveExists(naturalReserve.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(naturalReserve);
        }

        // GET: NaturalReserves/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var naturalReserve = await _context.NaturalReserves
                .FirstOrDefaultAsync(m => m.Id == id);
            if (naturalReserve == null)
            {
                return NotFound();
            }

            return View(naturalReserve);
        }

        // POST: NaturalReserves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var naturalReserve = await _context.NaturalReserves.FindAsync(id);
            if (naturalReserve != null)
            {
                naturalReserve.IsActive = false;
                naturalReserve.DeletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NaturalReserveExists(int id)
        {
            return _context.NaturalReserves.Any(e => e.Id == id);
        }
    }
}
