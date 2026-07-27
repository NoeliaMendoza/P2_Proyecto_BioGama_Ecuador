using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using BioGamaEcuador.Settings;
using MimeKit;

namespace BioGamaEcuador.Services;

public sealed class EmailService(IOptions<EmailSettings> options) : IEmailService
{
    private readonly EmailSettings _settings = options.Value;

    public async Task SendConfirmationEmailAsync(string to, string confirmationLink)
    {
        var body = EmailLayout("Confirma tu correo",
            "Gracias por registrarte en BioGama Ecuador. Para completar tu registro, confirma tu dirección de correo haciendo clic en el botón de abajo.",
            confirmationLink, "Confirmar cuenta");
        await SendAsync(to, "Confirma tu correo - BioGama Ecuador", body);
    }

    public async Task SendPasswordResetAsync(string to, string resetLink)
    {
        var body = EmailLayout("Restablece tu contraseña",
            "Recibimos una solicitud para restablecer la contraseña de tu cuenta. Haz clic en el botón de abajo para crear una nueva contraseña.",
            resetLink, "Restablecer contraseña");
        await SendAsync(to, "Restablece tu contraseña - BioGama Ecuador", body);
    }

    public async Task SendPasswordChangedAsync(string to)
    {
        var body = EmailLayout("Contraseña actualizada",
            "Tu contraseña ha sido cambiada exitosamente.",
            null, null, "Si no realizaste este cambio, contacta a soporte inmediatamente.");
        await SendAsync(to, "Contraseña actualizada - BioGama Ecuador", body);
    }

    public async Task SendAccountLockedAsync(string to, DateTime lockoutEnd)
    {
        var body = EmailLayout("Cuenta bloqueada",
            $"Tu cuenta ha sido bloqueada temporalmente por seguridad. Podrás intentar de nuevo a partir del <strong>{lockoutEnd:dd/MM/yyyy HH:mm}</strong>.",
            null, null, "Si necesitas ayuda, contacta a soporte.");
        await SendAsync(to, "Cuenta bloqueada - BioGama Ecuador", body);
    }

    public async Task SendMfaActivatedAsync(string to)
    {
        var body = EmailLayout("Autenticación de dos factores activada",
            "La autenticación de dos factores (MFA) ha sido activada en tu cuenta. A partir de ahora, al iniciar sesión se te solicitará un código adicional.",
            null, null, "Si no realizaste este cambio, contacta a soporte inmediatamente.");
        await SendAsync(to, "MFA activado - BioGama Ecuador", body);
    }

    public async Task SendOrderConfirmedAsync(string to, Guid orderId, decimal total)
    {
        var body = EmailLayout("Pedido confirmado",
            $"Tu pedido <strong>#{orderId}</strong> ha sido confirmado. Total pagado: <strong>${total:F2}</strong>",
            null, null, "Gracias por tu compra. Recibirás tu pedido en los próximos días.");
        await SendAsync(to, $"Pedido #{orderId} confirmado - BioGama Ecuador", body);
    }

    public async Task SendEnrollmentConfirmedAsync(string to, EnrollmentConfirmationInfo info)
    {
        var details = $"""
            <table style="width:100%;border-collapse:collapse;margin:16px 0;font-size:14px">
              <tr><td style="padding:8px 12px;background:#f8f9fa;font-weight:600;border:1px solid #dee2e6">Curso</td><td style="padding:8px 12px;border:1px solid #dee2e6">{info.CourseName}</td></tr>
              <tr><td style="padding:8px 12px;background:#f8f9fa;font-weight:600;border:1px solid #dee2e6">Modalidad</td><td style="padding:8px 12px;border:1px solid #dee2e6">{info.Modality}</td></tr>
              <tr><td style="padding:8px 12px;background:#f8f9fa;font-weight:600;border:1px solid #dee2e6">Lugar</td><td style="padding:8px 12px;border:1px solid #dee2e6">{info.Venue}</td></tr>
              <tr><td style="padding:8px 12px;background:#f8f9fa;font-weight:600;border:1px solid #dee2e6">Fecha inicio</td><td style="padding:8px 12px;border:1px solid #dee2e6">{info.StartDate:dd/MM/yyyy}</td></tr>
              <tr><td style="padding:8px 12px;background:#f8f9fa;font-weight:600;border:1px solid #dee2e6">Fecha fin</td><td style="padding:8px 12px;border:1px solid #dee2e6">{info.EndDate:dd/MM/yyyy}</td></tr>
              <tr><td style="padding:8px 12px;background:#f8f9fa;font-weight:600;border:1px solid #dee2e6">Horario</td><td style="padding:8px 12px;border:1px solid #dee2e6">{info.StartTime} - {info.EndTime}</td></tr>
              <tr><td style="padding:8px 12px;background:#f8f9fa;font-weight:600;border:1px solid #dee2e6">Instructor</td><td style="padding:8px 12px;border:1px solid #dee2e6">{info.Instructor}</td></tr>
              <tr><td style="padding:8px 12px;background:#f8f9fa;font-weight:600;border:1px solid #dee2e6">Codigo de confirmacion</td><td style="padding:8px 12px;border:1px solid #dee2e6"><strong>{info.ConfirmationCode}</strong></td></tr>
              <tr><td style="padding:8px 12px;background:#f8f9fa;font-weight:600;border:1px solid #dee2e6">Total pagado</td><td style="padding:8px 12px;border:1px solid #dee2e6"><strong>${info.TotalPaid:F2}</strong></td></tr>
            </table>
            <p style="margin:16px 0 0;color:#444;line-height:1.6;font-size:14px">Presenta tu codigo de confirmacion el dia del evento para registrar tu asistencia. Si tienes dudas, contacta a soporte.</p>
            """;

        var body = EmailLayout("Inscripcion confirmada",
            $"<p style='margin:0 0 8px;color:#444;line-height:1.6;font-size:15px'>Te has inscrito exitosamente en el curso <strong>{info.CourseName}</strong>. A continuacion los detalles:</p>{details}",
            null, null, "BioGama Ecuador - Todos los derechos reservados");
        await SendAsync(to, $"Inscripcion confirmada - {info.CourseName} - BioGama Ecuador", body);
    }

    public async Task SendPaymentFailedAsync(string to, Guid orderId, string reason)
    {
        var body = EmailLayout("Pago fallido",
            $"El pago de tu pedido <strong>#{orderId}</strong> no pudo ser procesado.",
            null, null, $"Motivo: {reason}. Puedes intentar nuevamente desde tu historial de pedidos.");
        await SendAsync(to, $"Pago fallido - Pedido #{orderId} - BioGama Ecuador", body);
    }

    public Task SendEmailAsync(string to, string subject, string body)
        => SendAsync(to, subject, body);

    public async Task SendLowStockAlertAsync(string to, string productName, int stock)
    {
        var body = EmailLayout("Alerta de stock bajo",
            $"El producto <strong>{productName}</strong> tiene stock crítico.",
            null, null, $"Unidades restantes: <strong>{stock}</strong>. Por favor, revisa el inventario y realiza los pedidos necesarios.");
        await SendAsync(to, $"Stock bajo: {productName} - BioGama Ecuador", body);
    }

    private static string EmailLayout(string title, string message, string? buttonUrl, string? buttonText, string? footer = null)
    {
        var button = buttonUrl != null && buttonText != null
            ? $"""<p style="text-align:center;margin:30px 0"><a href="{buttonUrl}" style="display:inline-block;padding:14px 32px;background-color:#198754;color:#fff;text-decoration:none;border-radius:6px;font-size:16px;font-weight:600">{buttonText}</a></p>"""
            : "";

        var footerHtml = footer != null
            ? $"""<p style="margin:16px 0 0;color:#888;font-size:13px">{footer}</p>"""
            : "";

        return $"""
            <!DOCTYPE html>
            <html>
            <head><meta charset="utf-8"></head>
            <body style="margin:0;padding:0;background-color:#f4f4f4;font-family:Segoe UI,Arial,sans-serif">
              <table width="100%" cellpadding="0" cellspacing="0">
                <tr><td style="padding:40px 10px">
                  <table width="600" cellpadding="0" cellspacing="0" style="margin:0 auto;background-color:#fff;border-radius:8px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,.08)">
                    <tr><td style="padding:30px 40px;background:linear-gradient(135deg,#2d6a4f,#40916c);text-align:center">
                      <h1 style="margin:0;color:#fff;font-size:24px">BioGama Ecuador</h1>
                    </td></tr>
                    <tr><td style="padding:40px">
                      <h2 style="margin:0 0 16px;color:#1a1a1a;font-size:20px">{title}</h2>
                      <p style="margin:0 0 16px;color:#444;line-height:1.6;font-size:15px">{message}</p>
                      {button}
                      {footerHtml}
                    </td></tr>
                    <tr><td style="padding:16px 40px;background-color:#f8f9fa;text-align:center;border-top:1px solid #eee">
                      <p style="margin:0;color:#999;font-size:12px">BioGama Ecuador &mdash; Todos los derechos reservados</p>
                    </td></tr>
                  </table>
                </td></tr>
              </table>
            </body>
            </html>
            """;
    }

    private async Task SendAsync(string to, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
