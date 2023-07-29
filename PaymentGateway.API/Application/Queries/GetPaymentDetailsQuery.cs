using MediatR;
using PaymentGateway.API.Application.Response;

namespace PaymentGateway.API.Application.Queries;

public class GetPaymentDetailsQuery : IRequest<PaymentDetailsResponse>
{
    public Guid PaymentId { get; init; }
}
