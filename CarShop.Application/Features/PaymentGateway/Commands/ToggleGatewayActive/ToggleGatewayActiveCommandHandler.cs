using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using PaymentGatewayEntity = CarShop.Domain.Entities.PaymentGateway;

namespace CarShop.Application.Features.PaymentGateway.Commands.ToggleGatewayActive
{
    public class ToggleGatewayActiveCommandHandler : IRequestHandler<ToggleGatewayActiveCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public ToggleGatewayActiveCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(ToggleGatewayActiveCommand request, CancellationToken cancellationToken)
        {
            var gateway = await _unitOfWork.Repository<PaymentGatewayEntity>().GetByIdAsync(request.Id);
            if (gateway == null) return Result<string>.Fail("Gateway not found.");

            var oldIsActive = gateway.IsActive;
            gateway.IsActive  = !gateway.IsActive;
            gateway.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<PaymentGatewayEntity>().Update(gateway);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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
    }
}
