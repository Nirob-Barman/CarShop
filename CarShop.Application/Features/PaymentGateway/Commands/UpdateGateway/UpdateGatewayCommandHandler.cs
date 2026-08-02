using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Payment;
using CarShop.Application.Wrappers;
using MediatR;
using PaymentGatewayEntity = CarShop.Domain.Entities.PaymentGateway;

namespace CarShop.Application.Features.PaymentGateway.Commands.UpdateGateway
{
    public class UpdateGatewayCommandHandler : IRequestHandler<UpdateGatewayCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfigEncryptor _encryptor;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public UpdateGatewayCommandHandler(
            IUnitOfWork unitOfWork,
            IConfigEncryptor encryptor,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _encryptor = encryptor;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(UpdateGatewayCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var newConfig = request.NewConfig;

            var gateway = await _unitOfWork.Repository<PaymentGatewayEntity>().GetByIdAsync(request.Id);
            if (gateway == null) return Result<string>.Fail("Gateway not found.");

            var oldValues = JsonSerializer.Serialize(new
            {
                gateway.Name, gateway.GatewayFamily, gateway.Type, gateway.IsActive,
                gateway.IsSandbox, gateway.SupportedCurrencies, gateway.SortOrder
            });

            gateway.Name                = dto.Name;
            gateway.GatewayFamily       = string.IsNullOrWhiteSpace(dto.GatewayFamily)
                ? GatewayConfigSchema.GetFamilyKey(gateway.Slug)
                : dto.GatewayFamily.ToLowerInvariant();
            gateway.Type                = dto.Type;
            gateway.LogoUrl             = dto.LogoUrl;
            gateway.IsActive            = dto.IsActive;
            gateway.IsSandbox           = dto.IsSandbox;
            gateway.SupportedCurrencies = dto.SupportedCurrencies;
            gateway.SortOrder           = dto.SortOrder;
            gateway.UpdatedAt           = DateTime.UtcNow;

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

            _unitOfWork.Repository<PaymentGatewayEntity>().Update(gateway);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("PaymentGateway", "Update",
                _userContextService.UserId, _userContextService.Email,
                $"Updated gateway '{gateway.Name}'",
                entityId: gateway.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues,
                newValues: JsonSerializer.Serialize(new
                {
                    gateway.Name, gateway.GatewayFamily, gateway.Type, gateway.IsActive,
                    gateway.IsSandbox, gateway.SupportedCurrencies, gateway.SortOrder
                }));

            return Result<string>.Ok(null, "Gateway updated successfully.");
        }
    }
}
