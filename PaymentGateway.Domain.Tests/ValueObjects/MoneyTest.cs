using FluentAssertions;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.Tests.ValueObjects;

public class MoneyTest
{
    [Theory]
    [InlineData(100, "USD", 100)]
    [InlineData(0, "USD", 0)]
    public void Create_ValidAmountAndCurrency(int amount, string currency, int expectedAmount)
    {
        var money = Money.Of(amount, currency);

        money.Amount.Should().Be(expectedAmount);
        money.Currency.Should().Be(currency);
    }

    [Theory]
    [InlineData(12345, "")]
    [InlineData(12345, null)]
    [InlineData(100, "XYZ")]
    public void ThrowException_GivenNullOrEmptyCurrency(int amount, string currency)
    {
        Action act = () => Money.Of(amount, currency);

        act.Should().Throw<MoneyDomainException>().WithMessage("Currency is invalid.");
    }

    [Theory]
    [InlineData(-100, "USD")]
    public void ThrowException_GivenInvalidAmount(int amount, string currency)
    {
        Action act = () => Money.Of(amount, currency);

        act.Should().Throw<MoneyDomainException>().WithMessage("Amount is invalid.");
    }
}
