using CarShop.Application.DTOs.Order;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Queries.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<Result<PagedResult<OrderDto>>>
    {
        public string? Status { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public GetAllOrdersQuery(string? status = null, int page = 1, int pageSize = 20)
        {
            Status = status;
            Page = page;
            PageSize = pageSize;
        }
    }
}
