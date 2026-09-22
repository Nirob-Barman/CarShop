using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.CreatePendingOrder
{
    public record CreatePendingOrderCommand(
        int CarId,
        string? PromoCode = null)
        : IRequest<Result<(int OrderId, decimal FinalPrice, string CarTitle)>>;
}
