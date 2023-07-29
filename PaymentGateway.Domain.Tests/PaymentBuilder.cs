using PaymentGateway.Domain;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.Tests;

public class PaymentBuilder
{
    private Guid _id;
    private Card _card;
    private Money _money;
    private DateTime _created;

    public PaymentBuilder()
    {
        _id = Guid.NewGuid();
        _card = new Card
        {
            CardHolderName = "Happy Shopper",
            CardNumber = CardNumber.Of("1234123412341234"),
            ExpiryDate = ExpiryDate.Of(12, 2025),
            CVV = CVV.Of("123")
        };
        _money = Money.Of(1000, "USD");
        _created = DateTime.Now;
    }

    public PaymentBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public PaymentBuilder WithCard(Card card)
    {
        _card = card;
        return this;
    }

    public PaymentBuilder WithMoney(Money money)
    {
        _money = money;
        return this;
    }

    public PaymentBuilder WithCreated(DateTime created)
    {
        _created = created;
        return this;
    }

    public Payment Build()
    {
        return new Payment
        {
            Id = _id,
            Card = _card,
            Money = _money,
            Created = _created
        };
    }
}