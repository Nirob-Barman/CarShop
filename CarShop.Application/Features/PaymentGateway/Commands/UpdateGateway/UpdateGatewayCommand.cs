using CarShop.Application.DTOs.Payment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Commands.UpdateGateway
{
    public record UpdateGatewayCommand(
        int Id,
        PaymentGatewayDto Dto,
        Dictionary<string, string>? NewConfig)
        : IRequest<Result<string>>;
}
