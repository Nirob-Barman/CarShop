using CarShop.Application.DTOs.Order;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Queries.GetAllOrders
{
    public record GetAllOrdersQuery(
        string? Status = null,
        int Page = 1,
        int PageSize = 20)
        : IRequest<Result<PagedResult<OrderDto>>>;
}
