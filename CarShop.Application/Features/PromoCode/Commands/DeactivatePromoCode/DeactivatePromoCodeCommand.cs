using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.DeactivatePromoCode
{
    public record DeactivatePromoCodeCommand(int Id)
        : IRequest<Result<string>>;
}
