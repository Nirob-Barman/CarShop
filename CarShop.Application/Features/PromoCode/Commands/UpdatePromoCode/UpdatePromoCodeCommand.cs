using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.UpdatePromoCode
{
    public record UpdatePromoCodeCommand(
        int Id,
        string Code,
        decimal DiscountPercent,
        decimal? MaxDiscountAmount,
        int? MaxUsages,
        DateTime? ExpiresAt
        ): IRequest<Result<string>>;
}
