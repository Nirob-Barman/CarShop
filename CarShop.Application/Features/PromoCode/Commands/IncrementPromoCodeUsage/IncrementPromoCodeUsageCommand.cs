using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.IncrementPromoCodeUsage
{
    public class IncrementPromoCodeUsageCommand : IRequest<Result<string>>
    {
        public int PromoCodeId { get; set; }

        public IncrementPromoCodeUsageCommand(int promoCodeId)
        {
            PromoCodeId = promoCodeId;
        }
    }
}
