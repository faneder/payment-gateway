using FluentAssertions;
using PaymentGateway.Domain;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.Tests;


public class CardTest
{
    [Fact]
    public void GetMaskedCardNumber_GivenCardNumber_ShouldReturnMaskedCardNumber()
    {
        var card = new Card
        {
            CardNumber = CardNumber.Of("1234123412347777")
        };

        var maskedCardNumber = card.GetMaskedCardNumber();

        maskedCardNumber.Should().Be("**** **** **** 7777");
    }
}