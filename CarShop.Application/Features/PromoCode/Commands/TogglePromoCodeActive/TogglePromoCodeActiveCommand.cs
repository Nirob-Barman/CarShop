using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.TogglePromoCodeActive
{
    public record TogglePromoCodeActiveCommand(int Id)
        : IRequest<Result<string>>;
}
