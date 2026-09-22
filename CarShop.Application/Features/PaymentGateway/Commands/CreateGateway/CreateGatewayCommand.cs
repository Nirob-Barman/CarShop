using CarShop.Application.DTOs.Payment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Commands.CreateGateway
{
    public record CreateGatewayCommand(
        PaymentGatewayDto Dto,
        Dictionary<string, string> Config)
        : IRequest<Result<string>>;
}
