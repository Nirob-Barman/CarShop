using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Queries.GetPromoCodeById
{
    public record GetPromoCodeByIdQuery(int Id)
        : IRequest<Result<PromoCodeDto>>;
}
