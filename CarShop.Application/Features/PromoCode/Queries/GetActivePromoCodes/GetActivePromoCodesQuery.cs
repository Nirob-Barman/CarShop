using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Queries.GetActivePromoCodes
{
    public class GetActivePromoCodesQuery : IRequest<Result<IEnumerable<PromoCodeDto>>>
    {
    }
}
