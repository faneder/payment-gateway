using MediatR;
using PaymentGateway.API.Application.Response;
using PaymentGateway.Domain.Interfaces;

namespace PaymentGateway.API.Application.Queries;

public class GetPaymentDetailsHandler : IRequestHandler<GetPaymentDetailsQuery, PaymentDetailsResponse>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentDetailsHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<PaymentDetailsResponse> Handle(GetPaymentDetailsQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetPaymentAsync(request.PaymentId);

        if (payment == null) return null;

        return new PaymentDetailsResponse
        {
            MaskedCardNumber = payment.Card.GetMaskedCardNumber(),
            CardHolderName = payment.Card.CardHolderName,
            CVV = payment.Card.CVV.Code,
            ExpiryDate = payment.Card.ExpiryDate.ToString(),
            Status = payment.Status
        };
    }
}