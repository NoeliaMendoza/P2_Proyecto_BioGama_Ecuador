using BioGamaEcuador.Data;
using BioGamaEcuador.Models.Admin;
using BioGamaEcuador.Models.Sales;
using BioGamaEcuador.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BioGamaEcuador.Controllers;

[Authorize(Roles = "Admin,Administrador")]
[Route("Admin/Courses")]
public sealed class AdminCoursesController(AppDbContext context, IAuditService audit) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? search, string? modality, string? status, DateTime? date, int page = 1)
    {
        const int pageSize = 12;
        var query = context.Courses.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(c => EF.Functions.ILike(c.Title, $"%{search.Trim()}%"));
        if (!string.IsNullOrWhiteSpace(modality)) query = query.Where(c => c.Modality == modality);
        if (date.HasValue) query = query.Where(c => c.StartDate.Date == date.Value.Date);
        if (status == "active") query = query.Where(c => c.IsActive && c.StartDate >= DateTime.Today);
        if (status == "inactive") query = query.Where(c => !c.IsActive);
        if (status == "finished") query = query.Where(c => c.EndDate < DateTime.Today);
        var total = await query.CountAsync();
        ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize)); ViewBag.Page = Math.Clamp(page, 1, (int)ViewBag.TotalPages);
        ViewBag.Search = search; ViewBag.Modality = modality; ViewBag.Status = status; ViewBag.Date = date?.ToString("yyyy-MM-dd");
        return View(await query.OrderByDescending(c => c.StartDate).Skip(((int)ViewBag.Page - 1) * pageSize).Take(pageSize).ToListAsync());
    }

    [HttpGet("Create")] public async Task<IActionResult> Create() { await SpeciesAsync(); return View(new CourseFormViewModel()); }
    [HttpPost("Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseFormViewModel model)
    {
        ValidateDates(model); if (!ModelState.IsValid) { await SpeciesAsync(); return View(model); }
        var course = new Course(); Apply(model, course); context.Courses.Add(course); await context.SaveChangesAsync();
        TempData["Success"] = "Curso creado correctamente."; return RedirectToAction(nameof(Index));
    }
    [HttpGet("Edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var c = await context.Courses.FindAsync(id); if (c is null) return NotFound(); await SpeciesAsync(); return View(ToForm(c));
    }
    [HttpPost("Edit/{id:guid}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CourseFormViewModel model)
    {
        if (id != model.Id) return BadRequest(); ValidateDates(model); var course = await context.Courses.SingleOrDefaultAsync(c => c.Id == id); if (course is null) return NotFound();
        if (!ModelState.IsValid) { await SpeciesAsync(); return View(model); }
        Apply(model, course);
        await context.SaveChangesAsync();
        TempData["Success"] = "Curso actualizado."; return RedirectToAction(nameof(Index));
    }
    [HttpPost("Delete/{id:guid}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    { var c = await context.Courses.SingleOrDefaultAsync(x => x.Id == id); if (c is null) return NotFound(); c.DeletedAt = DateTime.UtcNow; c.UpdatedAt = DateTime.UtcNow; await audit.LogAsync("SoftDelete", "Course", c.Id.ToString(), null, null, "system", null); await context.SaveChangesAsync(); TempData["Success"] = "Curso eliminado."; return RedirectToAction(nameof(Index)); }
    [HttpGet("Delete/{id:guid}")]
    public async Task<IActionResult> DeleteConfirmation(Guid id) { var course = await context.Courses.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id); return course is null ? NotFound() : View("Delete", course); }
    [HttpGet("Enrollments/{courseId:guid}")]
    public async Task<IActionResult> Enrollments(Guid courseId)
    { var course = await context.Courses.AsNoTracking().SingleOrDefaultAsync(c => c.Id == courseId); if (course is null) return NotFound(); ViewBag.Course = course; return View(await context.Enrollments.Include(e => e.User).Where(e => e.CourseId == courseId).OrderByDescending(e => e.EnrolledAt).ToListAsync()); }
    [HttpPost("Enrollments/{courseId:guid}/{id:guid}/CheckIn"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn(Guid courseId, Guid id)
    { var enrollment = await context.Enrollments.SingleOrDefaultAsync(e => e.Id == id && e.CourseId == courseId); if (enrollment is null) return NotFound(); enrollment.Status = "Attended"; enrollment.AttendedAt = DateTime.UtcNow; await context.SaveChangesAsync(); TempData["Success"] = "Asistencia registrada."; return RedirectToAction(nameof(Enrollments), new { courseId }); }
    private async Task SpeciesAsync() => ViewBag.SpeciesId = new SelectList(await context.Species.OrderBy(s => s.CommonName).ToListAsync(), "Id", "CommonName");
    private void ValidateDates(CourseFormViewModel m) { if (m.EndDate.Date < m.StartDate.Date) ModelState.AddModelError(nameof(m.EndDate), "La fecha final no puede ser anterior a la inicial."); if (m.EndDate.Date == m.StartDate.Date && m.EndTime <= m.StartTime) ModelState.AddModelError(nameof(m.EndTime), "La hora final debe ser posterior a la inicial."); }
    private static void Apply(CourseFormViewModel m, Course c) { c.Title=m.Title.Trim(); c.Description=m.Description.Trim(); c.Syllabus=m.Syllabus; c.Price=m.Price; c.TotalSeats=m.TotalSeats; c.StartDate=m.StartDate; c.EndDate=m.EndDate; c.StartTime=m.StartTime; c.EndTime=m.EndTime; c.Modality=m.Modality; c.Venue=m.Venue.Trim(); c.Instructor=m.Instructor.Trim(); c.InstructorBio=m.InstructorBio; c.ImageUrl=m.ImageUrl; c.IsActive=m.IsActive; c.RequiresPriorKnowledge=m.RequiresPriorKnowledge; c.TargetAudience=m.TargetAudience; c.SpeciesId=m.SpeciesId; c.UpdatedAt=DateTime.UtcNow; }
    private static CourseFormViewModel ToForm(Course c) => new() { Id=c.Id, Title=c.Title, Description=c.Description, Syllabus=c.Syllabus, Price=c.Price, TotalSeats=c.TotalSeats, StartDate=c.StartDate, EndDate=c.EndDate, StartTime=c.StartTime, EndTime=c.EndTime, Modality=c.Modality, Venue=c.Venue, Instructor=c.Instructor, InstructorBio=c.InstructorBio, ImageUrl=c.ImageUrl, IsActive=c.IsActive, RequiresPriorKnowledge=c.RequiresPriorKnowledge, TargetAudience=c.TargetAudience, SpeciesId=c.SpeciesId };
}
