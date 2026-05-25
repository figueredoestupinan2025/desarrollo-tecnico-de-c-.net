using Microsoft.Extensions.Options;

namespace ReservasFondoXYZ.Web.Services.Payments;

public sealed class UnconfiguredPaymentGateway : IPaymentGateway
{
    private readonly PaymentOptions _options;

    public UnconfiguredPaymentGateway(IOptions<PaymentOptions> options)
    {
        _options = options.Value;
    }

    public Task<PaymentResult> AuthorizeAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new PaymentResult(
            Approved: false,
            Provider: _options.Provider,
            Reference: $"UNCONFIGURED-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Message: $"Proveedor de pagos '{_options.Provider}' no configurado. Usa Payment:Provider='Mock' o configura una pasarela real."
        ));
    }
}

