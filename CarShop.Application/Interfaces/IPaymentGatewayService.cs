using CarShop.Application.DTOs.Payment;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface IPaymentGatewayService
    {
        Task<Result<IEnumerable<PaymentGatewayDto>>> GetAllAsync();
        Task<Result<IEnumerable<PaymentGatewayDto>>> GetActiveAsync();
        Task<Result<PaymentGatewayDto>> GetByIdAsync(int id);
        Task<Result<string>> CreateAsync(PaymentGatewayDto dto, Dictionary<string, string> config);
        Task<Result<string>> UpdateAsync(int id, PaymentGatewayDto dto, Dictionary<string, string>? newConfig);
        Task<Result<string>> ToggleActiveAsync(int id);
        Task<Result<string>> DeleteAsync(int id);
        Task<Dictionary<string, string>> GetDecryptedConfigAsync(int id);
    }
}
