using System.Globalization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Data.Seeders;
using BioGamaEcuador.Workers;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

static string? ReadSecret(string name)
{
    var path = $"/run/secrets/{name}";
    return File.Exists(path) ? File.ReadAllText(path).Trim() : null;
}

var builder = WebApplication.CreateBuilder(args);

var dbPassword = ReadSecret("db_password");
var emailPass = ReadSecret("email_password");
if (emailPass != null)
{
    builder.Configuration["EmailSettings:Password"] = emailPass;
}

var isMailWorker = string.Equals(
    builder.Configuration["SERVICE_TYPE"], "mailworker",
    StringComparison.OrdinalIgnoreCase);

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
if (dbPassword != null && connStr != null)
{
    var csb = new Npgsql.NpgsqlConnectionStringBuilder(connStr) { Password = dbPassword };
    connStr = csb.ConnectionString;
}

builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connStr!));

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>()
    .AddErrorDescriber<BioGamaEcuador.Services.SpanishIdentityErrorDescriber>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services
    .AddAuthentication();
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    builder.Services.AddAuthentication().AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
    });
}

// Configuración y Servicios de Pasarelas de Pago
builder.Services.Configure<BioGamaEcuador.Settings.PayPhoneSettings>(
    builder.Configuration.GetSection("PayPhone"));
builder.Services.Configure<BioGamaEcuador.Settings.PayPalSettings>(
    builder.Configuration.GetSection("PayPal"));

builder.Services.AddHttpClient<BioGamaEcuador.Services.Payments.PayPhoneApiLinkService>();
builder.Services.AddHttpClient<BioGamaEcuador.Services.Payments.PayPalService>();
builder.Services.AddScoped<BioGamaEcuador.Services.IInventoryService, BioGamaEcuador.Services.InventoryMovementService>();
builder.Services.AddScoped<BioGamaEcuador.Services.IEmailService, BioGamaEcuador.Services.EmailService>();
builder.Services.AddScoped<BioGamaEcuador.Services.IAuditService, BioGamaEcuador.Services.AuditService>();
builder.Services.AddScoped<BioGamaEcuador.Services.IPaymentGateway, BioGamaEcuador.Services.Payments.PayPalPaymentGateway>();
builder.Services.AddScoped<BioGamaEcuador.Services.IPaymentGateway, BioGamaEcuador.Services.Payments.PayPhonePaymentGateway>();
builder.Services.AddScoped<BioGamaEcuador.Services.IPaymentService, BioGamaEcuador.Services.PaymentService>();
builder.Services.AddScoped<BioGamaEcuador.Services.IAccountService, BioGamaEcuador.Services.AccountService>();
builder.Services.AddScoped<BioGamaEcuador.Services.IAIService, BioGamaEcuador.Services.AIService>();

builder.Services.Configure<BioGamaEcuador.Settings.EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Configuración y Servicio de IA Local Ollama
//builder.Services.Configure<BioGamaEcuador.Settings.OllamaSettings>(
//    builder.Configuration.GetSection("Ollama"));
//builder.Services.AddHttpClient<BioGamaEcuador.Services.Ollama.IOllamaService, BioGamaEcuador.Services.Ollama.OllamaService>();

// ── Compartir cookies de autenticación entre réplicas ──────
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/root/.aspnet/DataProtection-Keys"))
    .SetApplicationName("BioGamaEcuador");

// ── Sesiones distribuidas (PostgreSQL) ─────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ── Worker de correo (solo en modo mailworker) ─────────────
if (isMailWorker)
{
    builder.Services.AddHostedService<MailWorker>();
}

var app = builder.Build();

var cultura = new CultureInfo("es-EC");
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = new[] { cultura },
    SupportedUICultures = new[] { cultura },
    RequestCultureProviders = new List<IRequestCultureProvider> { new CustomRequestCultureProvider(_ => Task.FromResult(new ProviderCultureResult("es-EC"))) }
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Health check para Docker Swarm (sin DI para evitar errores de autenticación)
app.MapGet("/health", () =>
{
    return Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
});

// En modo mailworker no se inicia el servidor web
if (!isMailWorker)
{
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            await db.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogWarning(ex, "Migration failed (likely race condition between replicas), continuing...");
        }
        await IdentitySeeder.SeedAsync(scope.ServiceProvider);
        await SalesModuleSeeder.SeedAsync(scope.ServiceProvider);
    }

    app.Run();
}
else
{
    // En modo mailworker solo corre el BackgroundService
    app.Run();
}
