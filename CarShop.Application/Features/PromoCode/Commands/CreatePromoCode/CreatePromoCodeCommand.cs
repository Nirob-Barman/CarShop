using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.CreatePromoCode
{
    public record CreatePromoCodeCommand(
        string Code,
        decimal DiscountPercent,
        decimal? MaxDiscountAmount,
        int? MaxUsages,
        DateTime? ExpiresAt
        ) : IRequest<Result<string>>;
}
