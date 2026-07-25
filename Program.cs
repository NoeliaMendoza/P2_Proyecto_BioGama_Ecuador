using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Data.Seeders;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services
    .AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    });

// Configuración y Servicios de Pasarelas de Pago
builder.Services.Configure<BioGamaEcuador.Settings.PayPhoneSettings>(
    builder.Configuration.GetSection("PayPhone"));
builder.Services.Configure<BioGamaEcuador.Settings.PayPalSettings>(
    builder.Configuration.GetSection("PayPal"));

builder.Services.AddHttpClient<BioGamaEcuador.Services.Payments.PayPhoneApiLinkService>();
builder.Services.AddHttpClient<BioGamaEcuador.Services.Payments.PayPalService>();
builder.Services.AddScoped<BioGamaEcuador.Services.IInventoryMovementService, BioGamaEcuador.Services.InventoryMovementService>();
builder.Services.AddScoped<BioGamaEcuador.Services.IEmailService, BioGamaEcuador.Services.EmailService>();

// Configuración y Servicio de IA Local Ollama
builder.Services.Configure<BioGamaEcuador.Settings.OllamaSettings>(
    builder.Configuration.GetSection("Ollama"));
builder.Services.AddHttpClient<BioGamaEcuador.Services.Ollama.IOllamaService, BioGamaEcuador.Services.Ollama.OllamaService>();

var app = builder.Build();

var cultura = new CultureInfo("es-EC");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = new[] { cultura },
    SupportedUICultures = new[] { cultura }
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
    await SalesModuleSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
