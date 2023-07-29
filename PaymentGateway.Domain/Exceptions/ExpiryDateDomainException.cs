namespace PaymentGateway.Domain.Exceptions;

public class ExpiryDateDomainException : Exception
{
    public ExpiryDateDomainException(string message)
        : base(message)
    { }
}