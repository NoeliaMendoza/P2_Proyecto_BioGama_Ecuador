using BioGamaEcuador.Data;
using BioGamaEcuador.Services;
using Microsoft.EntityFrameworkCore;

namespace BioGamaEcuador.Workers;

public class MailWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MailWorker> _logger;

    public MailWorker(IServiceScopeFactory scopeFactory, ILogger<MailWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MailWorker iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var pending = await context.PendingEmails
                    .Where(e => !e.SentAt.HasValue && e.RetryCount < 5)
                    .OrderBy(e => e.CreatedAt)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                foreach (var email in pending)
                {
                    try
                    {
                        await emailService.SendEmailAsync(email.To, email.Subject, email.Body);
                        email.SentAt = DateTime.UtcNow;
                        _logger.LogInformation("Correo enviado a {To}", email.To);
                    }
                    catch (Exception ex)
                    {
                        email.RetryCount++;
                        email.LastError = ex.Message;
                        _logger.LogWarning(ex, "Error enviando correo a {To}", email.To);
                    }
                }

                await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ciclo de MailWorker");
            }

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}
