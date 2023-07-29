namespace Infrastructure.AcquiringBank;

public interface IAcquiringBankService
{
    Task<AcquiringBankResponse> ProcessPayment(AcquiringBankRequest request);
}
