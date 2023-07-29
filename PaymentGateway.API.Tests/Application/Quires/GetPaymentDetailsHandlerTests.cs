using FluentAssertions;
using Moq;
using PaymentGateway.API.Application.Queries;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Tests;
using Xunit;

namespace PaymentGateway.API.Tests.Application.Quires;

public class GetPaymentDetailsHandlerTests
{
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly GetPaymentDetailsHandler _handler;

    public GetPaymentDetailsHandlerTests()
    {
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _handler = new GetPaymentDetailsHandler(_mockPaymentRepository.Object);
    }

    [Fact]
    public async Task Handle_GivenValidPaymentId_ShouldReturnPaymentDetailsResult()
    {
        // Arrange
        var payment = new PaymentBuilder().Build();
        _mockPaymentRepository.Setup(repo => repo.GetPaymentAsync(payment.Id))
            .ReturnsAsync(payment);

        var query = new GetPaymentDetailsQuery
        {
            PaymentId = payment.Id
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.CardHolderName.Should().Be("Happy Shopper");
        result.ExpiryDate.Should().Be("12/2025");
        result.CVV.Should().Be("123");
        result.MaskedCardNumber.Should().Be("**** **** **** 1234");
        result.Status.Should().Be(PaymentStatus.Pending);
        _mockPaymentRepository.Verify(repo => repo.GetPaymentAsync(payment.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenInvalidPaymentId_ShouldReturnNull()
    {
        var paymentId = Guid.NewGuid();
        _mockPaymentRepository.Setup(repo => repo.GetPaymentAsync(paymentId))!
            .ReturnsAsync((Payment)null);

        var query = new GetPaymentDetailsQuery
        {
            PaymentId = paymentId
        };

        var handle = await _handler.Handle(query, CancellationToken.None);

        handle.Should().BeNull();
    }
}