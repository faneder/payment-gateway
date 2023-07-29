using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.Domain;

public class Payment
{
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public Card Card { get; set; } 
    public Money Money { get; set; }
    public PaymentStatus Status { get; set; } 
    public DateTime Created { get; set; }

    public Payment()
    {
        Id = Guid.NewGuid();
    }

    public void MarkAsAuthorized()
    {
        Status = PaymentStatus.Authorized;
    }

    public void MarkAsDeclined()
    {
        Status = PaymentStatus.Declined;
    }
}
