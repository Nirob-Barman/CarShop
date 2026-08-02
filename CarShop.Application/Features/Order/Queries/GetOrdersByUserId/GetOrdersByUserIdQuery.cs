using CarShop.Application.DTOs.Order;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Queries.GetOrdersByUserId
{
    public class GetOrdersByUserIdQuery : IRequest<Result<IEnumerable<OrderDto>>>
    {
    }
}
