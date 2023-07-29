using MongoDB.Driver;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Interfaces;

namespace Infrastructure.Persistence;

public class PaymentRepository : IPaymentRepository
{
    private readonly IMongoCollection<Payment> _payments;

    public PaymentRepository(IMongoClient client)
    {
        var database = client.GetDatabase("payment-gateway");
        _payments = database.GetCollection<Payment>("payments");
    }

    public async Task<Payment> GetPaymentAsync(Guid id)
    {
        return await _payments.Find(payment => payment.Id == id).FirstOrDefaultAsync();
    }

    public async Task SavePaymentAsync(Payment payment)
    {
        await _payments.InsertOneAsync(payment);
    }
}
