using CarShop.Application.DTOs.Order;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Queries.GetOrderById
{
    public record GetOrderByIdQuery(int OrderId)
        : IRequest<Result<OrderDto>>;
}
