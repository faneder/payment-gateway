using Infrastructure.AcquiringBank;
using PaymentGateway.Domain;

namespace AcquiringBankSimulator;

public class SimulatedAcquiringBankService : IAcquiringBankService
{
    public async Task<AcquiringBankResponse> ProcessPayment(AcquiringBankRequest request)
    {
        // Simulate the Acquiring Bank's response based on the payment amount
        var isSuccessful = request.Amount % 2 == 0;

        return await Task.FromResult(new AcquiringBankResponse
        {
            ResponseCode = isSuccessful ? "10000" : "20000",
            PaymentStatus = isSuccessful ? PaymentStatus.Authorized : PaymentStatus.Declined,
            ResponseMessage = isSuccessful ? "Payment was successful" : "Payment was declined", 
            TransactionId = Guid.NewGuid().ToString()
        });
    }
}