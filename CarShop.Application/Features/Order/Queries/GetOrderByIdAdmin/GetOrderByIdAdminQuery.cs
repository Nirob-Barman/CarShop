using CarShop.Application.DTOs.Order;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Queries.GetOrderByIdAdmin
{
    public record GetOrderByIdAdminQuery(int OrderId) 
        : IRequest<Result<OrderDto>>;
}
