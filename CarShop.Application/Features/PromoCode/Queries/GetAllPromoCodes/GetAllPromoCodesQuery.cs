using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Queries.GetAllPromoCodes
{
    public record GetAllPromoCodesQuery
        : IRequest<Result<IEnumerable<PromoCodeDto>>>;
}
