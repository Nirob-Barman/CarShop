using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Queries.GetCompletedOrdersCount
{
    public record GetCompletedOrdersCountQuery
        : IRequest<Result<int>>;
}
