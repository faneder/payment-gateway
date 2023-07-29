namespace PaymentGateway.Domain.Exceptions;

public class MoneyDomainException : Exception
{
    public MoneyDomainException(string message)
        : base(message)
    { }
}
