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
    public class ResearchersController : Controller
    {
        private readonly AppDbContext _context;

        public ResearchersController(AppDbContext context)
        {
            _context = context;
        }

        private static DateTime ToUtcKind(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
        }

        // GET: Researchers
        [Authorize(Roles = "Administrador,Investigador,UsuarioPublico")]
        public async Task<IActionResult> Index(string nombre, string institucion, int pagina = 1)
        {
            const int tamano = 50;
            var query = _context.Researchers.AsNoTracking().Where(r => r.IsActive);
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(r => EF.Functions.ILike(r.Name, $"%{nombre}%"));
            }
            if (!string.IsNullOrWhiteSpace(institucion))
            {
                query = query.Where(r => EF.Functions.ILike(r.Institution, $"%{institucion}%"));
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

            ViewBag.Nombre = nombre;
            ViewBag.Institucion = institucion;
            return View(datos);
        }

        // GET: Researchers/Details/5
        [Authorize(Roles = "Administrador,Investigador,UsuarioPublico")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var researcher = await _context.Researchers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (researcher == null)
            {
                return NotFound();
            }

            return View(researcher);
        }

        // GET: Researchers/Create
        [Authorize(Roles = "Administrador,Investigador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Researchers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Create([Bind("Id,Name,Institution,Email,Specialty,IsActive,CreatedAt")] Researcher researcher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(researcher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(researcher);
        }

        // GET: Researchers/Edit/5
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var researcher = await _context.Researchers.FindAsync(id);
            if (researcher == null)
            {
                return NotFound();
            }
            return View(researcher);
        }

        // POST: Researchers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Investigador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Institution,Email,Specialty,IsActive,CreatedAt")] Researcher researcher)
        {
            if (id != researcher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var created = researcher.CreatedAt;
                    if (created.Kind == DateTimeKind.Local)
                    {
                        created = created.ToUniversalTime();
                    }
                    else if (created.Kind == DateTimeKind.Unspecified)
                    {
                        created = DateTime.SpecifyKind(created, DateTimeKind.Utc);
                    }
                    researcher.CreatedAt = created;

                    _context.Update(researcher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResearcherExists(researcher.Id))
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
            return View(researcher);
        }

        // GET: Researchers/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var researcher = await _context.Researchers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (researcher == null)
            {
                return NotFound();
            }

            return View(researcher);
        }

        // POST: Researchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var researcher = await _context.Researchers.FindAsync(id);
            if (researcher != null)
            {
                researcher.IsActive = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResearcherExists(int id)
        {
            return _context.Researchers.Any(e => e.Id == id);
        }
    }
}
