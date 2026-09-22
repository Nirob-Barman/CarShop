using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.UpdatePromoCode
{
    public record UpdatePromoCodeCommand(int Id, PromoCodeDto Dto)
        : IRequest<Result<string>>;
}
