using PaymentGateway.Domain;

namespace PaymentGateway.API.Application.Response;

public class PaymentProcessedResponse
{
    public Guid PaymentId { get; init; }
    public PaymentStatus Status { get; init; }
    public string? ResponseCode { get; init; }
    public string? ResponseMessage { get; init; }
}