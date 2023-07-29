using FluentAssertions;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.Tests.ValueObjects;

public class CvvTest
{
    [Theory]
    [InlineData("123")]
    [InlineData("777")]
    public void CreateCVV_GivenValidCode(string code)
    {
        var cvv = CVV.Of(code);

        cvv.Code.Should().Be(code);
    }

    [Theory]
    [InlineData("12")]
    [InlineData("4567")]
    [InlineData("abc")]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowException_GivenInvalidCode(string code)
    {
        Action act = () => CVV.Of(code);

        act.Should().Throw<InvalidCVVDomainException>();
    }
}
