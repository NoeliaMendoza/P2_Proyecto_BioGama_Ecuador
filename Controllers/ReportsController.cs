using BioGamaEcuador.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioGamaEcuador.Controllers;

[Authorize(Roles = "Administrador,Investigador")]
public class ReportsController : Controller
{
    private readonly AppDbContext _context;

    public ReportsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var totalEspecies = await _context.Species.CountAsync(s => s.IsActive);
        var totalRegistros = await _context.Records.CountAsync(r => r.IsActive);
        var totalInvestigadores = await _context.Researchers.CountAsync(r => r.IsActive);
        var totalReservas = await _context.NaturalReserves.CountAsync(r => r.IsActive);

        ViewBag.Resumen = new
        {
            TotalEspecies = totalEspecies,
            TotalRegistros = totalRegistros,
            TotalInvestigadores = totalInvestigadores,
            TotalReservas = totalReservas
        };

        var registrosPorRegion = await _context.Records
            .AsNoTracking()
            .Where(r => r.IsActive)
            .Include(r => r.Location!)
                .ThenInclude(l => l.NaturalReserve)
            .GroupBy(r => r.Location!.NaturalReserve!.Region)
            .Select(g => new { Region = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        ViewBag.RegistrosPorRegion = registrosPorRegion;

        var especiesPorRegion = await _context.Records
            .AsNoTracking()
            .Where(r => r.IsActive)
            .Include(r => r.Location!)
                .ThenInclude(l => l.NaturalReserve)
            .GroupBy(r => r.Location!.NaturalReserve!.Region)
            .Select(g => new { Region = g.Key, EspeciesDistintas = g.Select(r => r.SpeciesId).Distinct().Count() })
            .OrderByDescending(x => x.EspeciesDistintas)
            .ToListAsync();

        ViewBag.EspeciesPorRegion = especiesPorRegion;

        var topEspecies = await _context.Records
            .AsNoTracking()
            .Where(r => r.IsActive)
            .Include(r => r.Species)
            .GroupBy(r => r.Species!.CommonName)
            .Select(g => new { Especie = g.Key, TotalRegistros = g.Count() })
            .OrderByDescending(x => x.TotalRegistros)
            .Take(10)
            .ToListAsync();

        ViewBag.TopEspecies = topEspecies;

        var especiesPorEstado = await _context.Species
            .AsNoTracking()
            .Where(s => s.IsActive && s.ConservationStatus != null)
            .GroupBy(s => new { s.ConservationStatus!.Code, s.ConservationStatus.Name })
            .Select(g => new { g.Key.Code, g.Key.Name, Total = g.Count() })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        ViewBag.EspeciesPorEstado = especiesPorEstado;

        var topInvestigadores = await _context.Records
            .AsNoTracking()
            .Where(r => r.IsActive)
            .Include(r => r.Researcher)
            .GroupBy(r => new { r.Researcher!.Name, r.Researcher.Institution })
            .Select(g => new { g.Key.Name, g.Key.Institution, TotalRegistros = g.Count() })
            .OrderByDescending(x => x.TotalRegistros)
            .Take(10)
            .ToListAsync();

        ViewBag.TopInvestigadores = topInvestigadores;

        var registrosPorAnio = await _context.Records
            .AsNoTracking()
            .Where(r => r.IsActive)
            .GroupBy(r => r.DiscoveryDate.Year)
            .Select(g => new { Anio = g.Key, Total = g.Count() })
            .OrderBy(x => x.Anio)
            .ToListAsync();

        ViewBag.RegistrosPorAnio = registrosPorAnio;

        return View();
    }
}