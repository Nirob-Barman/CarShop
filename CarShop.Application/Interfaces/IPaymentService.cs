using CarShop.Application.DTOs.Payment;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<string>> InitiatePaymentAsync(
            int carId, int gatewayId, string? promoCode,
            string successUrl, string cancelUrl);

        Task<Result<string>> HandleSuccessAsync(int transactionDbId, string gatewaySlug, string? sessionRefOverride = null);
        Task<Result<string>> HandleCancelAsync(int transactionDbId);
        Task<Result<PaymentTransactionDto>> GetTransactionByIdAsync(int transactionDbId);
    }
}
