namespace ReservasFondoXYZ.Web.Services.Payments;

public interface IPaymentGateway
{
    Task<PaymentResult> AuthorizeAsync(PaymentRequest request, CancellationToken cancellationToken = default);
}

