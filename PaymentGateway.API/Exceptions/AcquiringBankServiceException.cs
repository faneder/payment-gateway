namespace PaymentGateway.API.Exceptions;

public class AcquiringBankServiceException : Exception
{
    public AcquiringBankServiceException(string message) : base(message)
    {
        
    }
}