namespace ReservasFondoXYZ.Web.Services.Payments;

public sealed class MockPaymentGateway : IPaymentGateway
{
    public Task<PaymentResult> AuthorizeAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        var reference = $"MOCK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}";

        return Task.FromResult(new PaymentResult(
            Approved: true,
            Provider: "Mock",
            Reference: reference,
            Message: "Pago simulado aprobado."));
    }
}

