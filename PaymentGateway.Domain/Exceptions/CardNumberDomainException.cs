namespace PaymentGateway.Domain.Exceptions;

public class CardNumberDomainException : Exception
{
    public CardNumberDomainException(string message)
        : base(message)
    { }
}