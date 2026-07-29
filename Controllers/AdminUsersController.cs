using BioGamaEcuador.Models.Admin;
using BioGamaEcuador.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioGamaEcuador.Controllers;

[Authorize(Roles = "Admin,Administrador")]
[Route("Admin/Users")]
public sealed class AdminUsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IAuditService _audit;

    public AdminUsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IAuditService audit)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _audit = audit;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
        var list = new List<UserListViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            list.Add(new UserListViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles
            });
        }
        return View(list);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();

        return View(new UserEditViewModel
        {
            UserId = user.Id,
            Email = user.Email ?? "",
            Roles = allRoles.Select(r => new RoleCheckItem
            {
                RoleName = r.Name ?? "",
                IsAssigned = userRoles.Contains(r.Name, StringComparer.OrdinalIgnoreCase)
            }).ToList()
        });
    }

    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(string id, UserEditViewModel model)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);
        var selectedRoles = model.Roles.Where(r => r.IsAssigned).Select(r => r.RoleName).ToList();

        var rolesToAdd = selectedRoles.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToList();
        var rolesToRemove = currentRoles.Except(selectedRoles, StringComparer.OrdinalIgnoreCase).ToList();

        var changedBy = _userManager.GetUserId(User) ?? "";
        foreach (var role in rolesToAdd)
        {
            await _userManager.AddToRoleAsync(user, role);
            await _audit.LogRoleChangeAsync(user.Id, "", role, changedBy);
        }
        foreach (var role in rolesToRemove)
        {
            await _userManager.RemoveFromRoleAsync(user, role);
            await _audit.LogRoleChangeAsync(user.Id, role, "", changedBy);
        }

        TempData["Success"] = "Roles actualizados correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
