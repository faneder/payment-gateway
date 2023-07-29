using MediatR;
using PaymentGateway.API.Application.Response;

namespace PaymentGateway.API.Application.Commands;

public class ProcessPaymentCommand : IRequest<PaymentProcessedResponse>
{
    public Guid MerchantId { get; set; }
    public CardSource CardSource { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public PaymentType? PaymentType { get; set; }
    public string? Reference { get; set; }
    public DateTime Created { get; private set; }

    public ProcessPaymentCommand()
    {
        Created = DateTime.UtcNow;
    }
}

public class CardSource
{
    public string Name { get; set; }
    public string Number { get; set; }
    public string CVV { get; set; }   
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
}

public enum PaymentType
{
    Regular
}