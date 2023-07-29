using PaymentGateway.API.Application.Commands;

namespace PaymentGateway.API.Tests;

public class ProcessPaymentCommandBuilder
{
    private Guid _merchantId = Guid.NewGuid();
    private CardSource _cardSource = new()
    {
        Name = "Happy Shopper",
        Number = "4242424242424242",
        CVV = "123",
        ExpiryMonth = 12,
        ExpiryYear = 2023
    };
    private string _currency = "USD";
    private int _amount = 1000;
    private PaymentType _paymentType = PaymentType.Regular;
    private string _reference = "ORD-12345";

    public ProcessPaymentCommandBuilder WithCurrency(string currency)
    {
        
        _currency = currency;
        return this;
    }

    public ProcessPaymentCommand Build()
    {
        return new ProcessPaymentCommand()
        {
            MerchantId = _merchantId,
            CardSource = _cardSource,
            Currency = _currency,
            Amount = _amount,
            PaymentType = _paymentType,
            Reference = _reference
        };
    }
}