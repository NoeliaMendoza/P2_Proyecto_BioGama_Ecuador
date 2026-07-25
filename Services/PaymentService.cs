using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models.Sales;

namespace BioGamaEcuador.Services;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly IEnumerable<IPaymentGateway> _gateways;
    private readonly IAuditService _audit;

    public PaymentService(AppDbContext context, IEnumerable<IPaymentGateway> gateways, IAuditService audit)
    {
        _context = context;
        _gateways = gateways;
        _audit = audit;
    }

    private IPaymentGateway ResolveGateway(string provider)
    {
        var gateway = _gateways.FirstOrDefault(g =>
            g.ProviderName.Equals(provider, StringComparison.OrdinalIgnoreCase));
        return gateway ?? throw new InvalidOperationException($"No hay gateway registrado para el proveedor '{provider}'.");
    }

    public async Task<PaymentStartResult> InitiatePaymentAsync(Guid orderId, string provider, string returnUrl, string cancelUrl)
    {
        var order = await _context.Orders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null) throw new InvalidOperationException("Orden no encontrada.");

        var gateway = ResolveGateway(provider);

        var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
        if (existingPayment != null && existingPayment.Status == "Approved")
        {
            return new PaymentStartResult { Success = true, TransactionId = existingPayment.ExternalId };
        }

        var request = new PaymentRequest
        {
            OrderReference = order.Id.ToString(),
            Amount = order.Total,
            Currency = "USD",
            Description = $"Orden #{order.Id.ToString()[..8]}",
            ReturnUrl = returnUrl,
            CancelUrl = cancelUrl
        };

        var result = await gateway.CreatePaymentAsync(request);
        if (!result.Success)
            return result;

        if (existingPayment == null)
        {
            existingPayment = new Payment
            {
                OrderId = order.Id,
                Provider = gateway.ProviderName,
                ExternalId = result.TransactionId,
                Amount = order.Total,
                Currency = "USD",
                Status = "Pending",
                GatewayResponse = result.ApprovalUrl
            };
            _context.Payments.Add(existingPayment);
        }
        else
        {
            existingPayment.ExternalId = result.TransactionId;
            existingPayment.GatewayResponse = result.ApprovalUrl;
            existingPayment.Status = "Pending";
        }

        await _context.SaveChangesAsync();
        await _audit.LogAsync("PaymentInitiated", "Payment", existingPayment.Id.ToString(), null, "Pending", "system", null);
        return result;
    }

    public async Task<PaymentVerificationResult> VerifyPaymentAsync(Guid orderId, string provider, string transactionId)
    {
        var gateway = ResolveGateway(provider);
        var result = await gateway.VerifyPaymentAsync(transactionId);

        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId && p.ExternalId == transactionId);
        if (payment != null)
        {
            payment.Status = result.Success ? "Approved" : "Failed";
            if (result.Success) payment.ConfirmedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return result;
    }

    public async Task<bool> CancelPaymentAsync(Guid orderId, string provider, string transactionId)
    {
        var gateway = ResolveGateway(provider);
        var result = await gateway.CancelPaymentAsync(transactionId);

        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId && p.ExternalId == transactionId);
        if (payment != null)
        {
            payment.Status = "Cancelled";
            await _context.SaveChangesAsync();
        }

        return result.Success;
    }

    public async Task<IReadOnlyList<PaymentSummary>> GetPaymentsByOrderAsync(Guid orderId)
    {
        return await _context.Payments
            .Where(p => p.OrderId == orderId)
            .Select(p => new PaymentSummary
            {
                Id = p.Id,
                OrderId = p.OrderId,
                Provider = p.Provider,
                ExternalId = p.ExternalId,
                Amount = p.Amount,
                Currency = p.Currency,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                ConfirmedAt = p.ConfirmedAt
            })
            .ToListAsync();
    }
}
