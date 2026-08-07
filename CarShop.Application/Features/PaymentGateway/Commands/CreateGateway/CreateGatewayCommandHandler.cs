using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using PaymentGatewayEntity = CarShop.Domain.Entities.PaymentGateway;

namespace CarShop.Application.Features.PaymentGateway.Commands.CreateGateway
{
    public class CreateGatewayCommandHandler : IRequestHandler<CreateGatewayCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfigEncryptor _encryptor;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public CreateGatewayCommandHandler(
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

        public async Task<Result<string>> Handle(CreateGatewayCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var config = request.Config;

            var gateway = new PaymentGatewayEntity
            {
                Name                = dto.Name,
                GatewayFamily       = string.IsNullOrWhiteSpace(dto.GatewayFamily)
                    ? GatewayConfigSchema.GetFamilyKey(dto.Slug)
                    : dto.GatewayFamily.ToLowerInvariant(),
                Slug                = dto.Slug.ToLower(),
                Type                = dto.Type,
                LogoUrl             = dto.LogoUrl,
                IsActive            = dto.IsActive,
                IsSandbox           = dto.IsSandbox,
                SupportedCurrencies = dto.SupportedCurrencies,
                SortOrder           = dto.SortOrder,
                Config              = config.Count > 0 ? _encryptor.Encrypt(JsonSerializer.Serialize(config)) : null,
                CreatedAt           = DateTime.UtcNow,
                UpdatedAt           = DateTime.UtcNow
            };

            await _unitOfWork.Repository<PaymentGatewayEntity>().AddAsync(gateway);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("PaymentGateway", "Create",
                _userContextService.UserId, _userContextService.Email,
                $"Added gateway '{gateway.Name}' (slug: {gateway.Slug})",
                entityId: gateway.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                newValues: JsonSerializer.Serialize(new
                {
                    gateway.Name, gateway.GatewayFamily, gateway.Slug, gateway.Type, gateway.IsActive,
                    gateway.IsSandbox, gateway.SupportedCurrencies, gateway.SortOrder
                }));

            return Result<string>.Ok(null, "Gateway created successfully.");
        }
    }
}
