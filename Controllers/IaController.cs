using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;

namespace BioGamaEcuador.Controllers
{
    public class IaController : Controller
    {
        private readonly AppDbContext _context;

        public IaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var speciesList = await _context.Species
                .Include(s => s.ConservationStatus)
                .Where(s => s.IsActive)
                .OrderBy(s => s.CommonName)
                .ToListAsync();

            return View(speciesList);
        }
    }
}
