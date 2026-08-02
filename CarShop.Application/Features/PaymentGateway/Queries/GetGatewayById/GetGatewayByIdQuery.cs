using CarShop.Application.DTOs.Payment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Queries.GetGatewayById
{
    public class GetGatewayByIdQuery : IRequest<Result<PaymentGatewayDto>>
    {
        public int Id { get; set; }

        public GetGatewayByIdQuery(int id)
        {
            Id = id;
        }
    }
}
