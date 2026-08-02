using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.UpdatePromoCode
{
    public class UpdatePromoCodeCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
        public PromoCodeDto Dto { get; set; }

        public UpdatePromoCodeCommand(int id, PromoCodeDto dto)
        {
            Id = id;
            Dto = dto;
        }
    }
}
