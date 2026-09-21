using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.PromoCode.Commands.DeactivatePromoCode
{
    public class DeactivatePromoCodeCommandHandler : IRequestHandler<DeactivatePromoCodeCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICacheService _cacheService;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        private const string ActiveCodesKey = "promos:active";

        public DeactivatePromoCodeCommandHandler(
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

        public async Task<Result<string>> Handle(DeactivatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var promo = await _context.PromoCodes.FindAsync(request.Id);
            if (promo == null)
                return Result<string>.Fail("Promo code not found.");

            var oldIsActive = promo.IsActive;
            promo.Deactivate();
            _context.PromoCodes.Update(promo);
            await _context.SaveChangesAsync(cancellationToken);

            var redis = await _context.IntegrationSettings.Where(s => s.ServiceName == "Redis")
                .Select(s => new { s.IsEnabled }).FirstOrDefaultAsync(cancellationToken);
            if (redis != null && redis.IsEnabled)
                await _cacheService.RemoveAsync(ActiveCodesKey);

            await _auditLogService.LogAsync("PromoCode", "Deactivate",
                _userContextService.UserId, _userContextService.Email,
                $"Code '{promo.Code}' deactivated",
                entityId: promo.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: JsonSerializer.Serialize(new { IsActive = oldIsActive }),
                newValues: JsonSerializer.Serialize(new { IsActive = false }));

            return Result<string>.Ok(null, "Promo code deactivated.");
        }
    }
}
