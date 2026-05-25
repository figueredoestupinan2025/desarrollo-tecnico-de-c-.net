namespace ReservasFondoXYZ.Web.Services.Payments;

public sealed record PaymentRequest(
    decimal Amount,
    string Currency,
    string CardNumber,
    string CardHolderName,
    string Expiration,
    string Cvv,
    string Description);

public sealed record PaymentResult(
    bool Approved,
    string Provider,
    string Reference,
    string? Message = null);

