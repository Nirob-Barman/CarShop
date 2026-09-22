using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.UpdatePromoCode
{
    public class UpdatePromoCodeCommandHandler : IRequestHandler<UpdatePromoCodeCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public UpdatePromoCodeCommandHandler(
            IApplicationDbContext context,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _context = context;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(UpdatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var promo = await _context.PromoCodes.FindAsync(request.Id);
            if (promo == null) return Result<string>.Fail("Promo code not found.");

            var oldValues = JsonSerializer.Serialize(new
            {
                promo.DiscountPercent, promo.MaxDiscountAmount,
                promo.MaxUsages, promo.ExpiresAt
            });
            
            try
            {
                promo.Update(request.DiscountPercent, request.MaxDiscountAmount, request.MaxUsages, request.ExpiresAt);
                _context.PromoCodes.Update(promo);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Failed to update promo code: {ex.Message}");
            }

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
