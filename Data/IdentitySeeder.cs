using Microsoft.AspNetCore.Identity;

namespace BioGamaEcuador.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = { "Administrador", "Investigador", "UsuarioPublico" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Usuario Administrador
        string emailAdmin = "admin@biogama.ec";
        string passwordAdmin = "Admin123*";

        var admin = await userManager.FindByEmailAsync(emailAdmin);
        if (admin == null)
        {
            admin = new IdentityUser { UserName = emailAdmin, Email = emailAdmin, EmailConfirmed = true };
            await userManager.CreateAsync(admin, passwordAdmin);
            await userManager.AddToRoleAsync(admin, "Administrador");
        }

        // Usuario Investigador de prueba
        string emailInv = "investigador@biogama.ec";
        var inv = await userManager.FindByEmailAsync(emailInv);
        if (inv == null)
        {
            inv = new IdentityUser { UserName = emailInv, Email = emailInv, EmailConfirmed = true };
            await userManager.CreateAsync(inv, "Invest123*");
            await userManager.AddToRoleAsync(inv, "Investigador");
        }

        // Usuario Público de prueba (registrado, solo lectura)
        string emailPub = "usuario@biogama.ec";
        var pub = await userManager.FindByEmailAsync(emailPub);
        if (pub == null)
        {
            pub = new IdentityUser { UserName = emailPub, Email = emailPub, EmailConfirmed = true };
            await userManager.CreateAsync(pub, "Usuario123*");
            await userManager.AddToRoleAsync(pub, "UsuarioPublico");
        }
    }
}