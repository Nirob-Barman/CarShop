using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.PromoCode.Commands.UpdatePromoCode
{
    public class UpdatePromoCodeCommandHandler : IRequestHandler<UpdatePromoCodeCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICacheService _cacheService;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        private const string ActiveCodesKey = "promos:active";

        public UpdatePromoCodeCommandHandler(
            IApplicationDbContext context,
            ICacheService cacheService,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _context = context;
            _cacheService = cacheService;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(UpdatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var promo = await _context.PromoCodes.FindAsync(request.Id);
            if (promo == null) return Result<string>.Fail("Promo code not found.");

            var oldValues = JsonSerializer.Serialize(new
            {
                promo.DiscountPercent, promo.MaxDiscountAmount,
                promo.MaxUsages, promo.ExpiresAt
            });

            promo.DiscountPercent    = dto.DiscountPercent;
            promo.MaxDiscountAmount  = dto.MaxDiscountAmount;
            promo.MaxUsages          = dto.MaxUsages;
            promo.ExpiresAt          = dto.ExpiresAt;

            _context.PromoCodes.Update(promo);
            await _context.SaveChangesAsync(cancellationToken);

            var redis = await _context.IntegrationSettings.Where(s => s.ServiceName == "Redis")
                .Select(s => new { s.IsEnabled }).FirstOrDefaultAsync(cancellationToken);
            if (redis != null && redis.IsEnabled)
                await _cacheService.RemoveAsync(ActiveCodesKey);

            await _auditLogService.LogAsync("PromoCode", "Update",
                _userContextService.UserId, _userContextService.Email,
                $"Updated code '{promo.Code}'",
                entityId: promo.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues,
                newValues: JsonSerializer.Serialize(new
                {
                    promo.DiscountPercent, promo.MaxDiscountAmount,
                    promo.MaxUsages, promo.ExpiresAt
                }));

            return Result<string>.Ok(null, "Promo code updated.");
        }
    }
}
