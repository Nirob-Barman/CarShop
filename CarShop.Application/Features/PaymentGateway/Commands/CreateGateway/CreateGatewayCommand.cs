using CarShop.Application.DTOs.Payment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Commands.CreateGateway
{
    public class CreateGatewayCommand : IRequest<Result<string>>
    {
        public PaymentGatewayDto Dto { get; set; }
        public Dictionary<string, string> Config { get; set; }

        public CreateGatewayCommand(PaymentGatewayDto dto, Dictionary<string, string> config)
        {
            Dto = dto;
            Config = config;
        }
    }
}
