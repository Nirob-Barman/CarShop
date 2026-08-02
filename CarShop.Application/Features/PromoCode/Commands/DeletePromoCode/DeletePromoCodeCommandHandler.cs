using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;
using PromoCodeEntity = CarShop.Domain.Entities.PromoCode;

namespace CarShop.Application.Features.PromoCode.Commands.DeletePromoCode
{
    public class DeletePromoCodeCommandHandler : IRequestHandler<DeletePromoCodeCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        private const string ActiveCodesKey = "promos:active";

        public DeletePromoCodeCommandHandler(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(DeletePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var promo = await _unitOfWork.Repository<PromoCodeEntity>().GetByIdAsync(request.Id);
            if (promo == null)
                return Result<string>.Fail("Promo code not found.");

            var oldValues = JsonSerializer.Serialize(new
            {
                promo.Code, promo.DiscountPercent, promo.MaxDiscountAmount,
                promo.MaxUsages, promo.UsageCount, promo.ExpiresAt, promo.IsActive
            });

            _unitOfWork.Repository<PromoCodeEntity>().Remove(promo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var redis = await _unitOfWork.Repository<IntegrationSetting>()
                .FirstOrDefaultAsync(s => s.ServiceName == "Redis", s => new { s.IsEnabled });
            if (redis != null && redis.IsEnabled)
                await _cacheService.RemoveAsync(ActiveCodesKey);

            await _auditLogService.LogAsync("PromoCode", "Delete",
                _userContextService.UserId, _userContextService.Email,
                $"Deleted code '{promo.Code}'",
                entityId: request.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues);

            return Result<string>.Ok(null, "Promo code deleted.");
        }
    }
}
