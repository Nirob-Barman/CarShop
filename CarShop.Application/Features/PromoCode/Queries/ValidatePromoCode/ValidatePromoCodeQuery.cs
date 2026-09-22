using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Queries.ValidatePromoCode
{
    public record ValidatePromoCodeQuery(string Code)
        : IRequest<Result<ValidatePromoCodeResult>>;
}
