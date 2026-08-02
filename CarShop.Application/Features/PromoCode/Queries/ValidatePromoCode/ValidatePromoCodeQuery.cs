using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Queries.ValidatePromoCode
{
    public class ValidatePromoCodeQuery : IRequest<Result<ValidatePromoCodeResult>>
    {
        public string Code { get; set; }

        public ValidatePromoCodeQuery(string code)
        {
            Code = code;
        }
    }
}
