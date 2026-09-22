using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.IncrementPromoCodeUsage
{
    public record IncrementPromoCodeUsageCommand(int PromoCodeId)
        : IRequest<Result<string>>;
}
