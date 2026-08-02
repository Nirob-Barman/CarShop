using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.CreatePromoCode
{
    public class CreatePromoCodeCommand : IRequest<Result<string>>
    {
        public PromoCodeDto Dto { get; set; }

        public CreatePromoCodeCommand(PromoCodeDto dto)
        {
            Dto = dto;
        }
    }
}
