namespace PaymentGateway.Domain.Exceptions;

public class InvalidCVVDomainException : Exception
{
    public InvalidCVVDomainException(string message)
        : base(message)
    { }
}