using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Domain.ValueObjects;

public record ExpiryDate
{
    public int ExpiryMonth { get; private set; }
    public int ExpiryYear { get; private set; }

    private ExpiryDate(int expiryMonth, int expiryYear)
    {
        ExpiryMonth = expiryMonth;
        ExpiryYear = expiryYear;
    }

    public static ExpiryDate Of(int expiryMonth, int expiryYear)
    {
        ValidateExpiryDate(expiryMonth, expiryYear);

        return new ExpiryDate(expiryMonth, expiryYear);
    }

    private static void ValidateExpiryDate(int expiryMonth, int expiryYear)
    {
        if (expiryMonth is < 1 or > 12)
            throw new ExpiryDateDomainException($"Invalid expiry month {expiryMonth}.");

        if (expiryYear < DateTime.UtcNow.Year)
            throw new ExpiryDateDomainException($"Card is expired {expiryMonth}, {expiryYear}.");
    }
    
    public override string ToString()
    {
        return $"{ExpiryMonth.ToString("D2")}/{ExpiryYear}";
    }
}
