namespace BioGamaEcuador.Services.Payments;

public class PayPhonePaymentGateway : IPaymentGateway
{
    private readonly PayPhoneApiLinkService _payPhone;

    public PayPhonePaymentGateway(PayPhoneApiLinkService payPhone)
    {
        _payPhone = payPhone;
    }

    public string ProviderName => "PayPhone";

    public async Task<PaymentStartResult> CreatePaymentAsync(PaymentRequest request)
    {
        var clientTxId = $"BIO-{Guid.NewGuid():N}"[..15];
        var url = await _payPhone.CreatePaymentLinkAsync(request.Amount, clientTxId, request.OrderReference);
        return new PaymentStartResult
        {
            Success = !string.IsNullOrEmpty(url),
            TransactionId = clientTxId,
            ApprovalUrl = url
        };
    }

    public Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId)
    {
        return Task.FromResult(new PaymentVerificationResult
        {
            Success = false,
            Status = "Unknown",
            ErrorMessage = "Verificacion directa no soportada por PayPhone API Link. Use el webhook para confirmar pagos."
        });
    }

    public Task<PaymentCancellationResult> CancelPaymentAsync(string transactionId)
    {
        return Task.FromResult(new PaymentCancellationResult
        {
            Success = false,
            ErrorMessage = "Cancelacion directa no soportada por PayPhone API Link. Los enlaces expiran automaticamente."
        });
    }
}
