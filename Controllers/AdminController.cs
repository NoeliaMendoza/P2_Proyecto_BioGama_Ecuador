using BioGamaEcuador.Data;
using BioGamaEcuador.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioGamaEcuador.Controllers;

[Authorize(Roles = "Admin,Administrador")]
[Route("Admin")]
public sealed class AdminController(AppDbContext context) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var month = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        return View(new AdminDashboardViewModel { ActiveCourses = await context.Courses.CountAsync(c => c.IsActive && c.StartDate >= DateTime.UtcNow), MonthlyEnrollments = await context.Enrollments.CountAsync(e => e.EnrolledAt >= month) });
    }
}
