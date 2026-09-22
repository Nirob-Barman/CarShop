using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.DeletePromoCode
{
    public record DeletePromoCodeCommand(int Id)
        : IRequest<Result<string>>;
}
