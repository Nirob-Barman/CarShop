using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using PromoCodeEntity = CarShop.Domain.Entities.PromoCode;

namespace CarShop.Application.Features.PromoCode.Commands.CreatePromoCode
{
    public class CreatePromoCodeCommandHandler : IRequestHandler<CreatePromoCodeCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public CreatePromoCodeCommandHandler(
            IApplicationDbContext context,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _context = context;
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
                CreatedAt = DateTime.UtcNow
            };

            await _context.PromoCodes.AddAsync(promo);
            await _context.SaveChangesAsync(cancellationToken);

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
