using Checkout.PaymentGateway.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentGateway.API.Application.Commands;
using PaymentGateway.API.Application.Queries;
using PaymentGateway.API.Application.Response;
using PaymentGateway.Domain;
using Xunit;

namespace PaymentGateway.API.Tests.Controller;

public class PaymentControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly PaymentController _controller;

    public PaymentControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new PaymentController(_mediatorMock.Object);
    }

    [Fact]
    public async Task ProcessPayment_ValidCommand_ReturnsOkResult()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var resultPayment = new PaymentProcessedResponse()
        {
            PaymentId = paymentId,
            Status = PaymentStatus.Authorized,
            ResponseCode = "10000",
            ResponseMessage = "Payment is approved"
        };
        var command = new ProcessPaymentCommandBuilder().Build();

        _mediatorMock.Setup(m => m.Send(It.IsAny<ProcessPaymentCommand>(), default)).ReturnsAsync(resultPayment);

        // Act
        var result = await _controller.ProcessPayment(command);

        // Assert
        var okResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnValue = okResult.Value.Should().BeOfType<PaymentProcessedResponse>().Subject;
        returnValue.Should().BeEquivalentTo(resultPayment);
        _mediatorMock.Verify(m => m.Send(It.IsAny<ProcessPaymentCommand>(), default), Times.Once);
    }
    
    [Fact]
    public async Task GetPaymentDetails_Returns_NotFound_When_PaymentDetails_Null()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<GetPaymentDetailsQuery>(), default))!
            .ReturnsAsync((PaymentDetailsResponse)null);

        var result = await _controller.GetPaymentDetails(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetPaymentDetails_Returns_Ok_When_PaymentDetails_Not_Null()
    {
        var paymentDetails = new PaymentDetailsResponse();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPaymentDetailsQuery>(), default))
            .ReturnsAsync(paymentDetails);

        var result = await _controller.GetPaymentDetails(Guid.NewGuid());

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(paymentDetails, okResult.Value);
    }
}