using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Queries.GetPromoCodeById
{
    public class GetPromoCodeByIdQuery : IRequest<Result<PromoCodeDto>>
    {
        public int Id { get; set; }

        public GetPromoCodeByIdQuery(int id)
        {
            Id = id;
        }
    }
}
