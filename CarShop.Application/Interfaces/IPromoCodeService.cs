using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface IPromoCodeService
    {
        Task<Result<ValidatePromoCodeResult>> ValidateCodeAsync(string code);
        Task<Result<string>> CreateCodeAsync(PromoCodeDto dto);
        Task<Result<IEnumerable<PromoCodeDto>>> GetAllCodesAsync();
        Task<Result<IEnumerable<PromoCodeDto>>> GetAllActiveCodesAsync();
        Task<Result<PromoCodeDto>> GetByIdAsync(int id);
        Task<Result<string>> UpdateCodeAsync(int id, PromoCodeDto dto);
        Task<Result<string>> ToggleActiveAsync(int id);
        Task<Result<string>> DeactivateCodeAsync(int id);
        Task<Result<string>> DeleteCodeAsync(int id);
        Task IncrementUsageAsync(int promoCodeId);
        Task<Result<PromoCodeDto?>> GetActivePromoCodeAsync();
    }
}
