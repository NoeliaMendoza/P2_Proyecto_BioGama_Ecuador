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
    public class LocationsController : Controller
    {
        private readonly AppDbContext _context;

        public LocationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Locations
        [Authorize(Roles = "Administrador,Investigador,UsuarioPublico")]
        public async Task<IActionResult> Index(string busqueda, string reserva, int pagina = 1)
        {
            const int tamano = 50;
            var query = _context.Locations.AsNoTracking().Include(l => l.NaturalReserve).Where(l => l.IsActive);
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(l => EF.Functions.ILike(l.PlaceName, $"%{busqueda}%"));
            }
            if (!string.IsNullOrWhiteSpace(reserva))
            {
                if (int.TryParse(reserva, out var reserveId))
                {
                    query = query.Where(l => l.NaturalReserveId == reserveId);
                }
            }

            int total = await query.CountAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling(total / (double)tamano);
            ViewBag.TotalItems = total;

            var datos = await query
                .OrderBy(l => l.Id)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();

            ViewBag.Busqueda = busqueda;
            var reservasList = await _context.NaturalReserves.Where(n => n.IsActive).OrderBy(n => n.Name).ToListAsync();
            ViewBag.Reservas = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(reservasList, "Id", "Name");
            ViewBag.ReservaFiltro = reserva;
            return View(datos);
        }

        // GET: Locations/Details/5
        [Authorize(Roles = "Administrador,Investigador,UsuarioPublico")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .Include(l => l.NaturalReserve)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // GET: Locations/Create
        [Authorize(Roles = "Administrador,Investigador")]
        public IActionResult Create()
        {
            ViewData["NaturalReserveId"] = new SelectList(_context.NaturalReserves, "Id", "Name");
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Create([Bind("Id,PlaceName,Altitude,Latitude,Longitude,NaturalReserveId,IsActive,CreatedAt")] Location location)
        {
            if (ModelState.IsValid)
            {
                _context.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NaturalReserveId"] = new SelectList(_context.NaturalReserves, "Id", "Name", location.NaturalReserveId);
            return View(location);
        }

        // GET: Locations/Edit/5
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            ViewData["NaturalReserveId"] = new SelectList(_context.NaturalReserves, "Id", "Name", location.NaturalReserveId);
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlaceName,Altitude,Latitude,Longitude,NaturalReserveId,IsActive,CreatedAt")] Location location)
        {
            if (id != location.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var created = location.CreatedAt;
                    if (created.Kind == DateTimeKind.Local)
                    {
                        created = created.ToUniversalTime();
                    }
                    else if (created.Kind == DateTimeKind.Unspecified)
                    {
                        created = DateTime.SpecifyKind(created, DateTimeKind.Utc);
                    }
                    location.CreatedAt = created;

                    _context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.Id))
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
            ViewData["NaturalReserveId"] = new SelectList(_context.NaturalReserves, "Id", "Name", location.NaturalReserveId);
            return View(location);
        }

        // GET: Locations/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .Include(l => l.NaturalReserve)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location != null)
            {
                location.IsActive = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.Id == id);
        }
    }
}
