using FluentAssertions;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.Tests.ValueObjects;

public class ExpiryDateTest
{
    [Theory]
    [InlineData(8, 2025)]
    public void Create_GivenValidExpiryDate(int expiryMonth, int expiryYear)
    {
        var expiryDate = ExpiryDate.Of(expiryMonth, expiryYear);

        expiryDate.ExpiryMonth.Should().Be(expiryMonth);
        expiryDate.ExpiryYear.Should().Be(expiryYear);
    }

    [Theory]
    [InlineData(0, 2025)]
    [InlineData(13, 2025)]
    public void ThrowException_GivenInvalidExpiryDate(int expiryMonth, int expiryYear)
    {
        Action act = () => ExpiryDate.Of(expiryMonth, expiryYear);

        act.Should().Throw<ExpiryDateDomainException>().WithMessage($"Invalid expiry month {expiryMonth}.");
    }

    [Theory]
    [InlineData(8, 2000)]
    public void ThrowException_GivenAnExpiredCard(int expiryMonth, int expiryYear)
    {
        Action act = () => ExpiryDate.Of(expiryMonth, expiryYear);

        act.Should().Throw<ExpiryDateDomainException>().WithMessage($"Card is expired {expiryMonth}, {expiryYear}.");
    }
    
    [Theory]
    [InlineData(12, 2024, "12/2024")]
    [InlineData(1, 2030, "01/2030")]
    public void ToString_ReturnsCorrectFormat_WhenExpiryDateProvided(int expiryMonth, int expiryYear, string expected)
    {
        var expiryDate = ExpiryDate.Of(expiryMonth, expiryYear);

        var result = expiryDate.ToString();

        result.Should().Be(expected);
    }
}