namespace BioGamaEcuador.Services;

public interface IEmailService
{
    Task SendConfirmationAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    public Task SendConfirmationAsync(string to, string subject, string body)
    {
        // Placeholder: enviar correo real (SMTP, SendGrid, etc.)
        Console.WriteLine($"[EMAIL] To: {to} | Subject: {subject}");
        return Task.CompletedTask;
    }
}
