using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Services;

namespace BioGamaEcuador.Controllers;

[Authorize]
public class MyCoursesController : Controller
{
    private readonly AppDbContext _context;
    private readonly IAuditService _audit;

    public MyCoursesController(AppDbContext context, IAuditService audit)
    {
        _context = context;
        _audit = audit;
    }

    public async Task<IActionResult> Index()
    {
        var userId = (await _context.Users
            .Where(u => u.UserName == User.Identity!.Name)
            .Select(u => u.Id)
            .FirstOrDefaultAsync())!;

        if (userId == null) return Challenge();

        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.UserId == userId && e.Status == "Confirmed")
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

        await _audit.LogAsync("ViewMyCourses", "Enrollment", userId, null, null, userId, HttpContext.Connection.RemoteIpAddress?.ToString());

        return View(enrollments);
    }
}
