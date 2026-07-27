using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models.Sales;
using BioGamaEcuador.Services;

namespace BioGamaEcuador.Controllers;

[Authorize]
public class MyCoursesController : Controller
{
    private readonly AppDbContext _context;
    private readonly IAuditService _audit;
    private readonly IInventoryService _inventory;
    private readonly IEmailService _email;

    public MyCoursesController(AppDbContext context, IAuditService audit, IInventoryService inventory, IEmailService email)
    {
        _context = context;
        _audit = audit;
        _inventory = inventory;
        _email = email;
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

        try { await _audit.LogAsync("ViewMyCourses", "Enrollment", userId, null, null, userId, HttpContext.Connection.RemoteIpAddress?.ToString()); } catch { }

        return View(enrollments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid enrollmentId)
    {
        var userId = (await _context.Users
            .Where(u => u.UserName == User.Identity!.Name)
            .Select(u => u.Id)
            .FirstOrDefaultAsync())!;

        if (userId == null) return Challenge();

        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.UserId == userId && e.Status == "Confirmed");

        if (enrollment == null) return NotFound();
        if (enrollment.Course!.StartDate <= DateTime.UtcNow)
        {
            TempData["Error"] = "No puedes cancelar un curso que ya ha iniciado.";
            return RedirectToAction(nameof(Index));
        }

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var course = enrollment.Course;
            course.ConfirmedSeats = Math.Max(0, course.ConfirmedSeats - 1);
            enrollment.Status = "Cancelled";

            _context.InventoryMovements.Add(new InventoryMovement
            {
                CourseId = course.Id, TipoMovimiento = "Cancelacion", Cantidad = 1,
                Referencia = $"Enrollment:{enrollment.Id}", UsuarioId = userId
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            TempData["Error"] = "Error al cancelar la inscripción.";
            return RedirectToAction(nameof(Index));
        }

        try { await _audit.LogAsync("EnrollmentCancelled", "Enrollment", enrollmentId.ToString(), "Confirmed", "Cancelled", userId, HttpContext.Connection.RemoteIpAddress?.ToString()); } catch { }

        TempData["Success"] = "Inscripción cancelada correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
