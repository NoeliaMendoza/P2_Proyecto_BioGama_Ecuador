namespace BioGamaEcuador.Services;

public class EnrollmentConfirmationInfo
{
    public string CourseName { get; set; } = string.Empty;
    public string Modality { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string ConfirmationCode { get; set; } = string.Empty;
    public decimal TotalPaid { get; set; }
    public Guid OrderId { get; set; }
}

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string to, string confirmationLink);
    Task SendPasswordResetAsync(string to, string resetLink);
    Task SendPasswordChangedAsync(string to);
    Task SendAccountLockedAsync(string to, DateTime lockoutEnd);
    Task SendMfaActivatedAsync(string to);
    Task SendOrderConfirmedAsync(string to, Guid orderId, decimal total);
    Task SendEnrollmentConfirmedAsync(string to, EnrollmentConfirmationInfo info);
    Task SendPaymentFailedAsync(string to, Guid orderId, string reason);
    Task SendLowStockAlertAsync(string to, string productName, int stock);
    Task SendEmailAsync(string to, string subject, string body);
}
