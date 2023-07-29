using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Domain.ValueObjects;

public record CardNumber
{
    public string Number { get; private set; }

    private CardNumber(string number)
    {
        Number = number;
    }

    public static CardNumber Of(string number)
    {
        if (number.Length != 16 || !number.All(char.IsDigit))
            throw new CardNumberDomainException("Card number is invalid.");
            
        return new CardNumber(number);
    }
}