using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Services;

namespace BioGamaEcuador.Controllers
{
    public class IaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _audit;

        public IaController(AppDbContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<IActionResult> Index()
        {
            var speciesList = await _context.Species
                .Include(s => s.ConservationStatus)
                .Where(s => s.IsActive)
                .OrderBy(s => s.CommonName)
                .ToListAsync();

            await _audit.LogAsync("AiExecution", "Species", null, null, $"Queried {speciesList.Count} species", "system", HttpContext.Connection.RemoteIpAddress?.ToString());
            return View(speciesList);
        }
    }
}
