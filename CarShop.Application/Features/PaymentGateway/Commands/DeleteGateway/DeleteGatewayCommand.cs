using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Commands.DeleteGateway
{
    public class DeleteGatewayCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }

        public DeleteGatewayCommand(int id)
        {
            Id = id;
        }
    }
}
