using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using System.Text.Json;

namespace CarShop.Application.Services
{
    public class PromoCodeService : IPromoCodeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public PromoCodeService(IUnitOfWork unitOfWork, IAuditLogService auditLogService, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<ValidatePromoCodeResult>> ValidateCodeAsync(string code)
        {
            var promo = await _unitOfWork.Repository<PromoCode>().FirstOrDefaultAsync(
                p => p.Code == code.ToUpper() && p.IsActive);

            if (promo == null)
                return Result<ValidatePromoCodeResult>.Fail("Invalid or inactive promo code.");

            if (promo.ExpiresAt.HasValue && promo.ExpiresAt.Value < DateTime.UtcNow)
                return Result<ValidatePromoCodeResult>.Fail("Promo code has expired.");

            if (promo.MaxUsages.HasValue && promo.UsageCount >= promo.MaxUsages.Value)
                return Result<ValidatePromoCodeResult>.Fail("Promo code usage limit reached.");

            return Result<ValidatePromoCodeResult>.Ok(new ValidatePromoCodeResult
            {
                IsValid = true,
                DiscountPercent = promo.DiscountPercent,
                MaxDiscountAmount = promo.MaxDiscountAmount,
                PromoCodeId = promo.Id,
                Message = $"{promo.DiscountPercent}% discount applied!"
            });
        }

        public async Task<Result<string>> CreateCodeAsync(PromoCodeDto dto)
        {
            var exists = await _unitOfWork.Repository<PromoCode>().AnyAsync(p => p.Code == dto.Code.ToUpper());
            if (exists)
                return Result<string>.Fail("A promo code with this code already exists.");

            var promo = new PromoCode
            {
                Code = dto.Code.ToUpper(),
                DiscountPercent = dto.DiscountPercent,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                MaxUsages = dto.MaxUsages,
                ExpiresAt = dto.ExpiresAt,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<PromoCode>().AddAsync(promo);
            await _unitOfWork.SaveChangesAsync();

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

        public async Task<Result<IEnumerable<PromoCodeDto>>> GetAllCodesAsync()
        {
            var codes = await _unitOfWork.Repository<PromoCode>().GetAllAsync();
            var dtos = codes.Select(p => new PromoCodeDto
            {
                Id = p.Id,
                Code = p.Code,
                DiscountPercent = p.DiscountPercent,
                MaxDiscountAmount = p.MaxDiscountAmount,
                MaxUsages = p.MaxUsages,
                UsageCount = p.UsageCount,
                ExpiresAt = p.ExpiresAt,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            });
            return Result<IEnumerable<PromoCodeDto>>.Ok(dtos);
        }

        public async Task<Result<IEnumerable<PromoCodeDto>>> GetAllActiveCodesAsync()
        {
            var now   = DateTime.UtcNow;
            var codes = await _unitOfWork.Repository<PromoCode>().GetAllAsync(
                p => p.IsActive &&
                     (!p.MaxUsages.HasValue || p.UsageCount < p.MaxUsages.Value) &&
                     (!p.ExpiresAt.HasValue || p.ExpiresAt.Value > now),
                p => new PromoCodeDto
                {
                    Id                = p.Id,
                    Code              = p.Code,
                    DiscountPercent   = p.DiscountPercent,
                    MaxDiscountAmount = p.MaxDiscountAmount,
                    MaxUsages         = p.MaxUsages,
                    UsageCount        = p.UsageCount,
                    ExpiresAt         = p.ExpiresAt,
                    IsActive          = p.IsActive
                });
            return Result<IEnumerable<PromoCodeDto>>.Ok(codes.OrderByDescending(p => p.DiscountPercent));
        }

        public async Task<Result<PromoCodeDto>> GetByIdAsync(int id)
        {
            var promo = await _unitOfWork.Repository<PromoCode>().GetByIdAsync(id);
            if (promo == null) return Result<PromoCodeDto>.Fail("Promo code not found.");
            return Result<PromoCodeDto>.Ok(new PromoCodeDto
            {
                Id = promo.Id, Code = promo.Code, DiscountPercent = promo.DiscountPercent,
                MaxDiscountAmount = promo.MaxDiscountAmount, MaxUsages = promo.MaxUsages,
                UsageCount = promo.UsageCount, ExpiresAt = promo.ExpiresAt,
                IsActive = promo.IsActive, CreatedAt = promo.CreatedAt
            });
        }

        public async Task<Result<string>> UpdateCodeAsync(int id, PromoCodeDto dto)
        {
            var promo = await _unitOfWork.Repository<PromoCode>().GetByIdAsync(id);
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

            _unitOfWork.Repository<PromoCode>().Update(promo);
            await _unitOfWork.SaveChangesAsync();

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

        public async Task<Result<string>> ToggleActiveAsync(int id)
        {
            var promo = await _unitOfWork.Repository<PromoCode>().GetByIdAsync(id);
            if (promo == null) return Result<string>.Fail("Promo code not found.");

            var oldIsActive = promo.IsActive;
            promo.IsActive = !promo.IsActive;
            _unitOfWork.Repository<PromoCode>().Update(promo);
            await _unitOfWork.SaveChangesAsync();

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

        public async Task<Result<string>> DeactivateCodeAsync(int id)
        {
            var promo = await _unitOfWork.Repository<PromoCode>().GetByIdAsync(id);
            if (promo == null)
                return Result<string>.Fail("Promo code not found.");

            var oldIsActive = promo.IsActive;
            promo.IsActive = false;
            _unitOfWork.Repository<PromoCode>().Update(promo);
            await _unitOfWork.SaveChangesAsync();

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

        public async Task<Result<string>> DeleteCodeAsync(int id)
        {
            var promo = await _unitOfWork.Repository<PromoCode>().GetByIdAsync(id);
            if (promo == null)
                return Result<string>.Fail("Promo code not found.");

            var oldValues = JsonSerializer.Serialize(new
            {
                promo.Code, promo.DiscountPercent, promo.MaxDiscountAmount,
                promo.MaxUsages, promo.UsageCount, promo.ExpiresAt, promo.IsActive
            });

            _unitOfWork.Repository<PromoCode>().Remove(promo);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("PromoCode", "Delete",
                _userContextService.UserId, _userContextService.Email,
                $"Deleted code '{promo.Code}'",
                entityId: id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues);

            return Result<string>.Ok(null, "Promo code deleted.");
        }

        public async Task IncrementUsageAsync(int promoCodeId)
        {
            var promo = await _unitOfWork.Repository<PromoCode>().GetByIdAsync(promoCodeId);
            if (promo != null)
            {
                promo.UsageCount++;
                _unitOfWork.Repository<PromoCode>().Update(promo);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<Result<PromoCodeDto?>> GetActivePromoCodeAsync()
        {
            var promo = await _unitOfWork.Repository<PromoCode>().FirstOrDefaultAsync(
                p => p.IsActive &&
                     (!p.MaxUsages.HasValue || p.UsageCount < p.MaxUsages.Value) &&
                     (!p.ExpiresAt.HasValue || p.ExpiresAt.Value > DateTime.UtcNow));

            if (promo == null)
                return Result<PromoCodeDto?>.Ok(null);

            return Result<PromoCodeDto?>.Ok(new PromoCodeDto
            {
                Id = promo.Id,
                Code = promo.Code,
                DiscountPercent = promo.DiscountPercent,
                MaxDiscountAmount = promo.MaxDiscountAmount,
                MaxUsages = promo.MaxUsages,
                ExpiresAt = promo.ExpiresAt,
                IsActive = promo.IsActive
            });
        }
    }
}
