using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.DeletePromoCode
{
    public class DeletePromoCodeCommandHandler : IRequestHandler<DeletePromoCodeCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public DeletePromoCodeCommandHandler(
            IApplicationDbContext context,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _context = context;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(DeletePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var promo = await _context.PromoCodes.FindAsync(request.Id);
            if (promo == null)
                return Result<string>.Fail("Promo code not found.");

            var oldValues = JsonSerializer.Serialize(new
            {
                promo.Code, promo.DiscountPercent, promo.MaxDiscountAmount,
                promo.MaxUsages, promo.UsageCount, promo.ExpiresAt, promo.IsActive
            });

            _context.PromoCodes.Remove(promo);
            await _context.SaveChangesAsync(cancellationToken);

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
