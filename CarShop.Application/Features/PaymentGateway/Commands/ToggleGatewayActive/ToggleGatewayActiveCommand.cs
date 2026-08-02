using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Commands.ToggleGatewayActive
{
    public class ToggleGatewayActiveCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }

        public ToggleGatewayActiveCommand(int id)
        {
            Id = id;
        }
    }
}
