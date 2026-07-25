namespace BioGamaEcuador.Services;

public interface IPaymentService
{
    Task<PaymentStartResult> InitiatePaymentAsync(Guid orderId, string provider, string returnUrl, string cancelUrl);
    Task<PaymentVerificationResult> VerifyPaymentAsync(Guid orderId, string provider, string transactionId);
    Task<bool> CancelPaymentAsync(Guid orderId, string provider, string transactionId);
    Task<IReadOnlyList<PaymentSummary>> GetPaymentsByOrderAsync(Guid orderId);
}

public class PaymentSummary
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}
