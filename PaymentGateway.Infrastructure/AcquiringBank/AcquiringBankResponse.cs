using PaymentGateway.Domain;

namespace Infrastructure.AcquiringBank;

public class AcquiringBankResponse  
{
    public PaymentStatus PaymentStatus { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseMessage { get; set; }
    public string TransactionId { get; set; }
}
