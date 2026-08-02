using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Queries.GetCompletedOrdersCount
{
    public class GetCompletedOrdersCountQuery : IRequest<Result<int>>
    {
    }
}
