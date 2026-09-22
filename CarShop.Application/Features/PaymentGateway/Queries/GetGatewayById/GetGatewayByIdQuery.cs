using CarShop.Application.DTOs.Payment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Queries.GetGatewayById
{
    public record GetGatewayByIdQuery(int Id)
        : IRequest<Result<PaymentGatewayDto>>;
}
