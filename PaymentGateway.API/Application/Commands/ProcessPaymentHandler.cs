using Infrastructure.AcquiringBank;
using MediatR;
using PaymentGateway.API.Application.Response;
using PaymentGateway.API.Exceptions;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.API.Application.Commands;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentProcessedResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IAcquiringBankService _acquiringBankService;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public ProcessPaymentCommandHandler(IPaymentRepository paymentRepository, IAcquiringBankService acquiringBankService, ILogger<ProcessPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _acquiringBankService = acquiringBankService;
        _logger = logger;
    }

    public async Task<PaymentProcessedResponse> Handle(ProcessPaymentCommand command, CancellationToken cancellationToken)
    {
        var paymentId = Guid.NewGuid();

        try
        {
            var bankResponse = await _acquiringBankService.ProcessPayment(MapToAcquiringBankRequest(command));

            var payment = MapToPayment(command, bankResponse.PaymentStatus, paymentId);

            await AddPayment(payment);

            return new PaymentProcessedResponse
            {
                PaymentId = paymentId,
                Status = bankResponse.PaymentStatus,
                ResponseCode = bankResponse.ResponseCode, 
                ResponseMessage = bankResponse.ResponseMessage, 
            };
        }
        catch (AcquiringBankServiceException e)
        {
            _logger.LogError( $"Error processing payment with acquiring bank: {paymentId}, amount: {command.Amount}");
            throw new AcquiringBankServiceException(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError( $"Error processing payment with paymentId: {paymentId}, amount: {command.Amount}");
            throw new PaymentProcessedException(e.Message);
        }
    }

    private async Task AddPayment(Payment payment)
    {
        if (payment.Status == PaymentStatus.Authorized)
        {
            payment.MarkAsAuthorized();
        }
        else
        {
            payment.MarkAsDeclined();
        }

        await _paymentRepository.SavePaymentAsync(payment);
    }

    private static Payment MapToPayment(ProcessPaymentCommand command, PaymentStatus paymentStatus, Guid paymentId)
    {
        return new Payment
        {
            Id = paymentId,
            MerchantId = command.MerchantId,
            Card = new Card
            {
                CardHolderName = command.CardSource.Name,
                CardNumber = CardNumber.Of(command.CardSource.Number),
                ExpiryDate = ExpiryDate.Of(command.CardSource.ExpiryMonth, command.CardSource.ExpiryYear),
                CVV = CVV.Of(command.CardSource.CVV),
            },
            Money = Money.Of(command.Amount, command.Currency),
            Status = paymentStatus,
            Created = command.Created
        };
    }

    private static AcquiringBankRequest MapToAcquiringBankRequest(ProcessPaymentCommand command)
    {
        return new AcquiringBankRequest
        {
            CardNumber = command.CardSource.Number,
            ExpiryMonth = command.CardSource.ExpiryMonth,
            ExpiryYear = command.CardSource.ExpiryYear,
            Name = command.CardSource.Name,
            CVV = command.CardSource.CVV,
            Amount = command.Amount,
            Currency = command.Currency
        };
    }
}

public class PaymentProcessedException : Exception
{
    public PaymentProcessedException(string message) : base(message)
    {
    }
}
