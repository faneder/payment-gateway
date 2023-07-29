using PaymentGateway.Domain;

namespace PaymentGateway.API.Application.Response;

public class PaymentDetailsResponse
{
    public string MaskedCardNumber { get; init; }
    public string CardHolderName { get; init; }
    public string CVV { get; init; }
    public string ExpiryDate { get; init; }
    public PaymentStatus Status { get; init; }
}