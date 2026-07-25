namespace BioGamaEcuador.Services;

public class PaymentRequest
{
    public string OrderReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Description { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}

public class PaymentStartResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string ApprovalUrl { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class PaymentVerificationResult
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CaptureId { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public string? ErrorMessage { get; set; }
}

public class PaymentCancellationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface IPaymentGateway
{
    string ProviderName { get; }
    Task<PaymentStartResult> CreatePaymentAsync(PaymentRequest request);
    Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId);
    Task<PaymentCancellationResult> CancelPaymentAsync(string transactionId);
}
