using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.API.Application.Queries;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Tests;
using Xunit;

namespace PaymentGateway.API.Tests.Application.Quires;

public class GetPaymentDetailsQueryHandlerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public GetPaymentDetailsQueryHandlerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsCorrectPaymentDetails()
    {
        using var scope = _factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Arrange
        var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
        var payment = new PaymentBuilder().Build();
        await paymentRepository.SavePaymentAsync(payment);

        var query = new GetPaymentDetailsQuery
        {
            PaymentId = payment.Id
        };

        // Act
        var paymentDetails = await mediator.Send(query);

        // Assert
        paymentDetails.Should().NotBeNull();
    }
}