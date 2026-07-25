namespace BioGamaEcuador.Services.Payments;

public class PayPalPaymentGateway : IPaymentGateway
{
    private readonly PayPalService _payPal;

    public PayPalPaymentGateway(PayPalService payPal)
    {
        _payPal = payPal;
    }

    public string ProviderName => "PayPal";

    public async Task<PaymentStartResult> CreatePaymentAsync(PaymentRequest request)
    {
        var result = await _payPal.CreateOrderAsync(request.Amount, request.OrderReference, request.ReturnUrl, request.CancelUrl);
        return new PaymentStartResult
        {
            Success = !string.IsNullOrEmpty(result.OrderId),
            TransactionId = result.OrderId,
            ApprovalUrl = result.ApprovalUrl
        };
    }

    public async Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId)
    {
        var result = await _payPal.CaptureOrderAsync(transactionId);
        return new PaymentVerificationResult
        {
            Success = result.Status == "COMPLETED",
            Status = result.Status,
            CaptureId = result.CaptureId
        };
    }

    public Task<PaymentCancellationResult> CancelPaymentAsync(string transactionId)
    {
        return Task.FromResult(new PaymentCancellationResult
        {
            Success = false,
            ErrorMessage = "Cancelacion directa no soportada por PayPal. Consulte la documentacion de PayPal para reembolsos."
        });
    }
}
