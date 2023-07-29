using FluentAssertions;
using PaymentGateway.Domain;

namespace PaymentGateway.Tests;

public class PaymentTests
{
    [Fact]
    public void MarkAsPending_PaymentIsInitialized()
    {
        var payment = new PaymentBuilder().Build();

        payment.Status.Should().Be(PaymentStatus.Pending);
    }
    
    [Fact]
    public void MarkAsAuthorize_PaymentIsAuthorized()
    {
        var payment = new PaymentBuilder().Build();

        payment.MarkAsAuthorized();

        payment.Status.Should().Be(PaymentStatus.Authorized);
    }
    
    [Fact]
    public void MarkAsDeclined_PaymentIsDeclined()
    {
        var payment = new PaymentBuilder().Build();

        payment.MarkAsDeclined();

        payment.Status.Should().Be(PaymentStatus.Declined);
    }
}