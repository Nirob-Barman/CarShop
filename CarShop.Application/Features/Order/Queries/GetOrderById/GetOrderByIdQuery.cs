using CarShop.Application.DTOs.Order;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<Result<OrderDto>>
    {
        public int OrderId { get; set; }

        public GetOrderByIdQuery(int orderId)
        {
            OrderId = orderId;
        }
    }
}
