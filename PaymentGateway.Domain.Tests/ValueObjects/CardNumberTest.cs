using FluentAssertions;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.Tests.ValueObjects;

public class CardNumberTest
{
    [Theory]
    [InlineData("4242424242424242")]
    [InlineData("1234567890123456")]
    public void Create_ValidCardNumber(string number)
    {
        var cardNumber = CardNumber.Of(number);

        cardNumber.Number.Should().Be(number);
    }

    [Theory]
    [InlineData("123456789012345")]
    [InlineData("12345678901234567")]
    [InlineData("abcdabcdabcdabcd")]
    public void InvalidCardNumber_ThrowException(string number)
    {
        Action act = () => CardNumber.Of(number);

        act.Should().Throw<CardNumberDomainException>().WithMessage("Card number is invalid.");
    }
}