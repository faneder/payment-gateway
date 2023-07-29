using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Interfaces;
using Xunit;

namespace PaymentGateway.API.Tests.Application.Commands;

public class ProcessPaymentCommandHandlerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ProcessPaymentCommandHandlerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Handle_ValidCommand_ProcessesPaymentSuccessfully()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();

        var command = new ProcessPaymentCommandBuilder().Build();

        var result = await mediator.Send(command);

        result.Status.Should().Be(PaymentStatus.Authorized);
        var paymentRepo = await paymentRepository.GetPaymentAsync(result.PaymentId);
        paymentRepo.Status.Should().Be(PaymentStatus.Authorized);
    }
}