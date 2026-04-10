using CarShop.Application.DTOs.Payment;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using System.Text.Json;

namespace CarShop.Application.Services
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfigEncryptor _encryptor;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public PaymentGatewayService(IUnitOfWork unitOfWork, IConfigEncryptor encryptor, IAuditLogService auditLogService, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _encryptor  = encryptor;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<PaymentGatewayDto>>> GetAllAsync()
        {
            var gateways = await _unitOfWork.Repository<PaymentGateway>()
                .GetAllAsync(_ => true, g => ToDto(g));
            return Result<IEnumerable<PaymentGatewayDto>>.Ok(gateways.OrderBy(g => g.SortOrder));
        }

        public async Task<Result<IEnumerable<PaymentGatewayDto>>> GetActiveAsync()
        {
            var gateways = await _unitOfWork.Repository<PaymentGateway>()
                .GetAllAsync(g => g.IsActive, g => ToDto(g));
            return Result<IEnumerable<PaymentGatewayDto>>.Ok(gateways.OrderBy(g => g.SortOrder));
        }

        public async Task<Result<PaymentGatewayDto>> GetByIdAsync(int id)
        {
            var gateway = await _unitOfWork.Repository<PaymentGateway>().GetByIdAsync(id);
            if (gateway == null) return Result<PaymentGatewayDto>.Fail("Gateway not found.");
            return Result<PaymentGatewayDto>.Ok(ToDto(gateway));
        }

        public async Task<Result<string>> CreateAsync(PaymentGatewayDto dto, Dictionary<string, string> config)
        {
            var exists = await _unitOfWork.Repository<PaymentGateway>()
                .AnyAsync(g => g.Slug == dto.Slug);
            if (exists) return Result<string>.Fail("A gateway with this slug already exists.");

            var gateway = new PaymentGateway
            {
                Name               = dto.Name,
                Slug               = dto.Slug.ToLower(),
                Type               = dto.Type,
                LogoUrl            = dto.LogoUrl,
                IsActive           = dto.IsActive,
                IsSandbox          = dto.IsSandbox,
                SupportedCurrencies = dto.SupportedCurrencies,
                SortOrder          = dto.SortOrder,
                Config             = config.Count > 0 ? _encryptor.Encrypt(JsonSerializer.Serialize(config)) : null,
                CreatedAt          = DateTime.UtcNow,
                UpdatedAt          = DateTime.UtcNow
            };

            await _unitOfWork.Repository<PaymentGateway>().AddAsync(gateway);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("PaymentGateway", "Create",
                _userContextService.UserId, _userContextService.Email,
                $"Added gateway '{gateway.Name}' (slug: {gateway.Slug})",
                entityId: gateway.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                newValues: JsonSerializer.Serialize(new
                {
                    gateway.Name, gateway.Slug, gateway.Type, gateway.IsActive,
                    gateway.IsSandbox, gateway.SupportedCurrencies, gateway.SortOrder
                }));

            return Result<string>.Ok(null, "Gateway created successfully.");
        }

        public async Task<Result<string>> UpdateAsync(int id, PaymentGatewayDto dto, Dictionary<string, string>? newConfig)
        {
            var gateway = await _unitOfWork.Repository<PaymentGateway>().GetByIdAsync(id);
            if (gateway == null) return Result<string>.Fail("Gateway not found.");

            var oldValues = JsonSerializer.Serialize(new
            {
                gateway.Name, gateway.Type, gateway.IsActive,
                gateway.IsSandbox, gateway.SupportedCurrencies, gateway.SortOrder
            });

            gateway.Name               = dto.Name;
            gateway.Type               = dto.Type;
            gateway.LogoUrl            = dto.LogoUrl;
            gateway.IsActive           = dto.IsActive;
            gateway.IsSandbox          = dto.IsSandbox;
            gateway.SupportedCurrencies = dto.SupportedCurrencies;
            gateway.SortOrder          = dto.SortOrder;
            gateway.UpdatedAt          = DateTime.UtcNow;

            if (newConfig != null && newConfig.Any(kv => !string.IsNullOrWhiteSpace(kv.Value)))
            {
                // Load existing config and merge — only overwrite keys that have a non-blank new value
                Dictionary<string, string> merged = [];
                if (gateway.Config != null)
                {
                    try
                    {
                        var existing = _encryptor.Decrypt(gateway.Config);
                        merged = JsonSerializer.Deserialize<Dictionary<string, string>>(existing) ?? [];
                    }
                    catch { }
                }
                foreach (var kv in newConfig)
                    if (!string.IsNullOrWhiteSpace(kv.Value))
                        merged[kv.Key] = kv.Value;

                gateway.Config = _encryptor.Encrypt(JsonSerializer.Serialize(merged));
            }

            _unitOfWork.Repository<PaymentGateway>().Update(gateway);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("PaymentGateway", "Update",
                _userContextService.UserId, _userContextService.Email,
                $"Updated gateway '{gateway.Name}'",
                entityId: gateway.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues,
                newValues: JsonSerializer.Serialize(new
                {
                    gateway.Name, gateway.Type, gateway.IsActive,
                    gateway.IsSandbox, gateway.SupportedCurrencies, gateway.SortOrder
                }));

            return Result<string>.Ok(null, "Gateway updated successfully.");
        }

        public async Task<Result<string>> ToggleActiveAsync(int id)
        {
            var gateway = await _unitOfWork.Repository<PaymentGateway>().GetByIdAsync(id);
            if (gateway == null) return Result<string>.Fail("Gateway not found.");

            var oldIsActive = gateway.IsActive;
            gateway.IsActive  = !gateway.IsActive;
            gateway.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<PaymentGateway>().Update(gateway);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("PaymentGateway", gateway.IsActive ? "Enable" : "Disable",
                _userContextService.UserId, _userContextService.Email,
                $"Gateway '{gateway.Name}' {(gateway.IsActive ? "enabled" : "disabled")}",
                entityId: gateway.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: JsonSerializer.Serialize(new { IsActive = oldIsActive }),
                newValues: JsonSerializer.Serialize(new { IsActive = gateway.IsActive }));

            return Result<string>.Ok(null, gateway.IsActive ? "Gateway enabled." : "Gateway disabled.");
        }

        public async Task<Result<string>> DeleteAsync(int id)
        {
            var gateway = await _unitOfWork.Repository<PaymentGateway>().GetByIdAsync(id);
            if (gateway == null) return Result<string>.Fail("Gateway not found.");

            var oldValues = JsonSerializer.Serialize(new
            {
                gateway.Name, gateway.Slug, gateway.Type, gateway.IsActive,
                gateway.IsSandbox, gateway.SupportedCurrencies, gateway.SortOrder
            });

            _unitOfWork.Repository<PaymentGateway>().Remove(gateway);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("PaymentGateway", "Delete",
                _userContextService.UserId, _userContextService.Email,
                $"Deleted gateway '{gateway.Name}'",
                entityId: id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues);

            return Result<string>.Ok(null, "Gateway deleted.");
        }

        public async Task<Dictionary<string, string>> GetDecryptedConfigAsync(int id)
        {
            var gateway = await _unitOfWork.Repository<PaymentGateway>().GetByIdAsync(id);
            if (gateway == null) return [];

            Dictionary<string, string> result = [];
            if (gateway.Config != null)
            {
                try
                {
                    var json = _encryptor.Decrypt(gateway.Config);
                    result = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? [];
                }
                catch { }
            }

            // Always inject sandbox flag so processors can use it without a separate DB call
            result["_is_sandbox"] = gateway.IsSandbox ? "true" : "false";
            return result;
        }

        private static PaymentGatewayDto ToDto(PaymentGateway g) => new()
        {
            Id                  = g.Id,
            Name                = g.Name,
            Slug                = g.Slug,
            Type                = g.Type,
            LogoUrl             = g.LogoUrl,
            IsActive            = g.IsActive,
            IsSandbox           = g.IsSandbox,
            SupportedCurrencies = g.SupportedCurrencies,
            SortOrder           = g.SortOrder,
            CreatedAt           = g.CreatedAt,
            UpdatedAt           = g.UpdatedAt
        };
    }
}
