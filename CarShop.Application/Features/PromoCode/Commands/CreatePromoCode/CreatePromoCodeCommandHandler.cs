using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;
using PromoCodeEntity = CarShop.Domain.Entities.PromoCode;

namespace CarShop.Application.Features.PromoCode.Commands.CreatePromoCode
{
    public class CreatePromoCodeCommandHandler : IRequestHandler<CreatePromoCodeCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        private const string ActiveCodesKey = "promos:active";

        public CreatePromoCodeCommandHandler(
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

        public async Task<Result<string>> Handle(CreatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var promo = new PromoCodeEntity
            {
                Code = dto.Code.ToUpper(),
                DiscountPercent = dto.DiscountPercent,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                MaxUsages = dto.MaxUsages,
                ExpiresAt = dto.ExpiresAt,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<PromoCodeEntity>().AddAsync(promo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var redis = await _unitOfWork.Repository<IntegrationSetting>()
                .FirstOrDefaultAsync(s => s.ServiceName == "Redis", s => new { s.IsEnabled });
            if (redis != null && redis.IsEnabled)
                await _cacheService.RemoveAsync(ActiveCodesKey);

            await _auditLogService.LogAsync("PromoCode", "Create",
                _userContextService.UserId, _userContextService.Email,
                $"Created code '{promo.Code}' ({promo.DiscountPercent}% off)",
                entityId: promo.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                newValues: JsonSerializer.Serialize(new
                {
                    promo.Code, promo.DiscountPercent, promo.MaxDiscountAmount,
                    promo.MaxUsages, promo.ExpiresAt, promo.IsActive
                }));

            return Result<string>.Ok(null, "Promo code created successfully.");
        }
    }
}
