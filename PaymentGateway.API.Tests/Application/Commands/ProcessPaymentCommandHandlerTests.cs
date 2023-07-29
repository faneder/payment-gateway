using FluentAssertions;
using Infrastructure.AcquiringBank;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.API.Application.Commands;
using PaymentGateway.API.Exceptions;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Interfaces;
using Xunit;

namespace PaymentGateway.API.Tests.Application.Commands;

public class ProcessPaymentCommandHandlerTests
{
    private readonly ProcessPaymentCommand _command = new ProcessPaymentCommandBuilder().Build();
    private static readonly string ErrorMessage = "Unexpected error";

    private static Mock<IAcquiringBankService> _mockAcquiringBankService;
    private static Mock<IPaymentRepository> _mockPaymentRepository;
    private static Mock<ILogger<ProcessPaymentCommandHandler>>? _mockLogger;

    public ProcessPaymentCommandHandlerTests()
    {
        _mockAcquiringBankService = new();
        _mockPaymentRepository = new();
        _mockLogger = new Mock<ILogger<ProcessPaymentCommandHandler>>();
    }

    [Fact]
    public async Task Handle_ValidPaymentRequest_PaymentProcessedSuccessfully()
    {
        var handler = GivenHandlerWithStatus(true);

        var response = await handler.Handle(_command, CancellationToken.None);

        response.PaymentId.Should().NotBe(Guid.Empty);
        response.Status.Should().Be(PaymentStatus.Authorized);
        response.ResponseCode.Should().Be("10000");
        response.ResponseMessage.Should().Be("Payment was successful");
        _mockPaymentRepository.Verify(repository => repository.SavePaymentAsync(It.Is<Payment>(p => p.Status == PaymentStatus.Authorized)), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidPaymentRequest_PaymentDeclined()
    {
        var handler = GivenHandlerWithStatus(false);

        var response = await handler.Handle(_command, CancellationToken.None);

        response.PaymentId.Should().NotBe(Guid.Empty);
        response.Status.Should().Be(PaymentStatus.Declined);
        response.ResponseCode.Should().Be("20000");
        response.ResponseMessage.Should().Be("Payment was declined");
    }

    [Fact]
    public async Task Handle_AcquiringBankServiceThrowsException_ErrorLoggedAndExceptionThrown()
    {
        var handler = GivenHandlerWithAcquiringBankException();

        Func<Task> act = async () => await handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<AcquiringBankServiceException>().WithMessage(ErrorMessage);
        _mockPaymentRepository.Verify(repository => repository.SavePaymentAsync(It.IsAny<Payment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PaymentProcessedThrowsException_ErrorLoggedAndExceptionThrown()
    {
        var handler = GivenHandlerWithStatus(true);
        var command = new ProcessPaymentCommandBuilder().WithCurrency("xxx").Build();

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<PaymentProcessedException>().WithMessage("Currency is invalid.");
        _mockPaymentRepository.Verify(repository => repository.SavePaymentAsync(It.IsAny<Payment>()), Times.Never);
    }

    private static ProcessPaymentCommandHandler GivenHandlerWithStatus(bool isSuccess)
    {
        _mockPaymentRepository
            .Setup(repository => repository.SavePaymentAsync(It.IsAny<Payment>()))
            .Returns(Task.CompletedTask);

        _mockAcquiringBankService
            .Setup(service => service.ProcessPayment(It.IsAny<AcquiringBankRequest>()))
            .ReturnsAsync(
                new AcquiringBankResponse
                {
                    PaymentStatus = isSuccess ? PaymentStatus.Authorized : PaymentStatus.Declined,
                    ResponseCode = isSuccess ? "10000" : "20000",
                    ResponseMessage = isSuccess ? "Payment was successful" : "Payment was declined"
                });

        return new ProcessPaymentCommandHandler(_mockPaymentRepository.Object, _mockAcquiringBankService.Object, _mockLogger.Object);
    }

    private static ProcessPaymentCommandHandler GivenHandlerWithAcquiringBankException()
    {
        _mockAcquiringBankService
            .Setup(service => service.ProcessPayment(It.IsAny<AcquiringBankRequest>()))
            .ThrowsAsync(new Exception(ErrorMessage));

        return new ProcessPaymentCommandHandler(_mockPaymentRepository.Object, _mockAcquiringBankService.Object, _mockLogger.Object);
    }
}
