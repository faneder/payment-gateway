using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Domain.ValueObjects;

public record Money
{
    public int Amount { get; private set; }
    public string Currency { get; private set; }

    private static readonly HashSet<string> ValidCurrencies = new()
    {
        "USD"
    };
    
    private Money(int amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Of(int amount, string currency)
    {
        if (string.IsNullOrEmpty(currency) || !ValidCurrencies.Contains(currency))
            throw new MoneyDomainException("Currency is invalid.");
        
        if (amount < 0)
            throw new MoneyDomainException("Amount is invalid.");

        return new Money(amount, currency);
    }
}
