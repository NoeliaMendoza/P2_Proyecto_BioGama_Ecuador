using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models;

namespace BioGamaEcuador.Controllers
{
    public class NaturalReservesController : Controller
    {
        private readonly AppDbContext _context;

        public NaturalReservesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: NaturalReserves
        public async Task<IActionResult> Index()
        {
            return View(await _context.NaturalReserves.ToListAsync());
        }

        // GET: NaturalReserves/Details/5
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: NaturalReserves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var naturalReserve = await _context.NaturalReserves.FindAsync(id);
            if (naturalReserve != null)
            {
                _context.NaturalReserves.Remove(naturalReserve);
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
