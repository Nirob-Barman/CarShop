using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.PromoCode.Commands.TogglePromoCodeActive
{
    public class TogglePromoCodeActiveCommandHandler : IRequestHandler<TogglePromoCodeActiveCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public TogglePromoCodeActiveCommandHandler(
            IApplicationDbContext context,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _context = context;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(TogglePromoCodeActiveCommand request, CancellationToken cancellationToken)
        {
            var promo = await _context.PromoCodes.FindAsync(request.Id);
            if (promo == null) return Result<string>.Fail("Promo code not found.");

            var oldIsActive = promo.IsActive;
            promo.ToggleActive();
            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("PromoCode", promo.IsActive ? "Activate" : "Deactivate",
                _userContextService.UserId, _userContextService.Email,
                $"Code '{promo.Code}' {(promo.IsActive ? "activated" : "deactivated")}",
                entityId: promo.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: JsonSerializer.Serialize(new { IsActive = oldIsActive }),
                newValues: JsonSerializer.Serialize(new { IsActive = promo.IsActive }));

            return Result<string>.Ok(null, promo.IsActive ? "Promo code activated." : "Promo code deactivated.");
        }
    }
}
