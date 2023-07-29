using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Domain.ValueObjects;

public record CVV 
{
    public string Code { get; private set; }
    private CVV(string code)
    {
        Code = code;
    }

    public static CVV Of(string code)
    {
        if (string.IsNullOrEmpty(code) || code.Length != 3 || !code.All(char.IsDigit))
            throw new InvalidCVVDomainException("CVV is invalid.");

        return new CVV(code);
    }
}