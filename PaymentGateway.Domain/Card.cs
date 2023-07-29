using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.Domain;

public record Card
{   
    public string CardHolderName { get; init; }
    public CardNumber CardNumber { get; init; }
    public ExpiryDate ExpiryDate { get; init; }
    public CVV CVV { get; init; }

    public string GetMaskedCardNumber()
    {
        return "**** **** **** " + CardNumber.Number.Substring(CardNumber.Number.Length - 4);
    }
}
