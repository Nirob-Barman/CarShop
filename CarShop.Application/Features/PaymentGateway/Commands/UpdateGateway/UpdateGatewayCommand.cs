using CarShop.Application.DTOs.Payment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Commands.UpdateGateway
{
    public class UpdateGatewayCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
        public PaymentGatewayDto Dto { get; set; }
        public Dictionary<string, string>? NewConfig { get; set; }

        public UpdateGatewayCommand(int id, PaymentGatewayDto dto, Dictionary<string, string>? newConfig)
        {
            Id = id;
            Dto = dto;
            NewConfig = newConfig;
        }
    }
}
