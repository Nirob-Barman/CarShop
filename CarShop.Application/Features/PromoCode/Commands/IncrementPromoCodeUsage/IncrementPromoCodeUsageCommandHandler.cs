using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;
using PromoCodeEntity = CarShop.Domain.Entities.PromoCode;

namespace CarShop.Application.Features.PromoCode.Commands.IncrementPromoCodeUsage
{
    public class IncrementPromoCodeUsageCommandHandler : IRequestHandler<IncrementPromoCodeUsageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        private const string ActiveCodesKey = "promos:active";

        public IncrementPromoCodeUsageCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<string>> Handle(IncrementPromoCodeUsageCommand request, CancellationToken cancellationToken)
        {
            var promo = await _unitOfWork.Repository<PromoCodeEntity>().GetByIdAsync(request.PromoCodeId);
            if (promo == null)
                return Result<string>.Fail("Promo code not found.");

            promo.UsageCount++;
            _unitOfWork.Repository<PromoCodeEntity>().Update(promo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var redis = await _unitOfWork.Repository<IntegrationSetting>()
                .FirstOrDefaultAsync(s => s.ServiceName == "Redis", s => new { s.IsEnabled });
            if (redis != null && redis.IsEnabled)
                await _cacheService.RemoveAsync(ActiveCodesKey);

            return Result<string>.Ok(null, "Usage incremented.");
        }
    }
}
