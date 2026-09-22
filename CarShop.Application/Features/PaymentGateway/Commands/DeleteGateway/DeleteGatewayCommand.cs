using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Commands.DeleteGateway
{
    public record DeleteGatewayCommand(int Id)
        : IRequest<Result<string>>;
}
