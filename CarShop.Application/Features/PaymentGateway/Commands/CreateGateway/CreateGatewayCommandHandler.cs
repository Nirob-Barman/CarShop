using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using PaymentGatewayEntity = CarShop.Domain.Entities.PaymentGateway;

namespace CarShop.Application.Features.PaymentGateway.Commands.CreateGateway
{
    public class CreateGatewayCommandHandler : IRequestHandler<CreateGatewayCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfigEncryptor _encryptor;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public CreateGatewayCommandHandler(
            IApplicationDbContext context,
            IConfigEncryptor encryptor,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _context = context;
            _encryptor = encryptor;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(CreateGatewayCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var config = request.Config;

            var gateway = PaymentGatewayEntity.Create(
                name: dto.Name,
                gatewayFamily: string.IsNullOrWhiteSpace(dto.GatewayFamily)
                    ? GatewayConfigSchema.GetFamilyKey(dto.Slug)
                    : dto.GatewayFamily,
                slug: dto.Slug,
                type: dto.Type,
                logoUrl: dto.LogoUrl,
                isActive: dto.IsActive,
                isSandbox: dto.IsSandbox,
                supportedCurrencies: dto.SupportedCurrencies,
                sortOrder: dto.SortOrder,
                config: config.Count > 0
                    ? _encryptor.Encrypt(JsonSerializer.Serialize(config))
                    : null);

            await _context.PaymentGateways.AddAsync(gateway);
            await _context.SaveChangesAsync(cancellationToken);

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
