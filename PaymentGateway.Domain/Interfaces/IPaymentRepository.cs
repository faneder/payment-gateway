namespace PaymentGateway.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment> GetPaymentAsync(Guid id);
    Task SavePaymentAsync(Payment payment);
}
