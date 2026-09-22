using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.CreatePromoCode
{
    public record CreatePromoCodeCommand(PromoCodeDto Dto)
        : IRequest<Result<string>>;
}
