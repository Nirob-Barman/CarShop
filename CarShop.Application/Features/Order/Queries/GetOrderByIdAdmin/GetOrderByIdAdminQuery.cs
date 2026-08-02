using CarShop.Application.DTOs.Order;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Queries.GetOrderByIdAdmin
{
    public class GetOrderByIdAdminQuery : IRequest<Result<OrderDto>>
    {
        public int OrderId { get; set; }

        public GetOrderByIdAdminQuery(int orderId)
        {
            OrderId = orderId;
        }
    }
}
