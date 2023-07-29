using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.API.Application.Commands;
using PaymentGateway.API.Application.Queries;

namespace Checkout.PaymentGateway.Controllers;

[Route("payments")]
public class PaymentController : Controller
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPaymentDetails), new { paymentId = result.PaymentId }, result);
    }

    [HttpGet("{PaymentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentDetails(Guid paymentId)
    {
        if (paymentId == Guid.Empty)
            return BadRequest();

        var paymentDetails = await _mediator.Send(new GetPaymentDetailsQuery { PaymentId = paymentId });

        if (paymentDetails == null)
            return NotFound();

        return Ok(paymentDetails);
    }
}