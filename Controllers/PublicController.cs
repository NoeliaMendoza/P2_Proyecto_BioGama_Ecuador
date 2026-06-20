using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models;

namespace BioGamaEcuador.Controllers
{
    public class PublicController : Controller
    {
        private readonly AppDbContext _context;

        public PublicController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Public/Catalogo
        // Vista pública del catálogo de especies (sin autenticación)
        public async Task<IActionResult> Catalogo(
            string? busqueda,
            string? familia,
            string? estado,
            bool? endemica,
            int pagina = 1)
        {
            const int tamano = 20;

            var query = _context.Species
                .AsNoTracking()
                .Include(s => s.Family)
                .Include(s => s.ConservationStatus)
                .Where(s => s.IsActive);

            // Filtro por búsqueda (nombre común o científico)
            if (!string.IsNullOrWhiteSpace(busqueda))
                query = query.Where(s =>
                    EF.Functions.ILike(s.CommonName, $"%{busqueda}%") ||
                    EF.Functions.ILike(s.ScientificName, $"%{busqueda}%"));

            // Filtro por familia
            if (!string.IsNullOrWhiteSpace(familia))
                query = query.Where(s => s.Family != null && s.Family.Name == familia);

            // Filtro por estado de conservación
            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(s => s.ConservationStatus != null && s.ConservationStatus.Code == estado);

            // Filtro por endémica
            if (endemica.HasValue)
                query = query.Where(s => s.IsEndemic == endemica.Value);

            int total = await query.CountAsync();

            var especies = await query
                .OrderBy(s => s.ScientificName)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();

            // Datos para los filtros dropdown
            ViewBag.Familias = await _context.Families
                .AsNoTracking()
                .Where(f => f.IsActive)
                .OrderBy(f => f.Name)
                .Select(f => f.Name)
                .ToListAsync();

            ViewBag.Estados = await _context.ConservationStatuses
                .AsNoTracking()
                .Where(cs => cs.IsActive)
                .OrderBy(cs => cs.Code)
                .Select(cs => cs.Code)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling(total / (double)tamano);
            ViewBag.TotalEspecies = total;
            ViewBag.Busqueda = busqueda;
            ViewBag.FamiliaFiltro = familia;
            ViewBag.EstadoFiltro = estado;
            ViewBag.EndémicaFiltro = endemica;

            return View(especies);
        }

        // GET: /Public/DetalleEspecie/5
        public async Task<IActionResult> DetalleEspecie(int? id)
        {
            if (id == null) return NotFound();

            var especie = await _context.Species
                .AsNoTracking()
                .Include(s => s.Family)
                .Include(s => s.ConservationStatus)
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (especie == null) return NotFound();

            return View(especie);
        }
    }
}
