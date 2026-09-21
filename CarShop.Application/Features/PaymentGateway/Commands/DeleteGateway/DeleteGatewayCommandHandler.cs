using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Commands.DeleteGateway
{
    public class DeleteGatewayCommandHandler : IRequestHandler<DeleteGatewayCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public DeleteGatewayCommandHandler(
            IApplicationDbContext context,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _context = context;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(DeleteGatewayCommand request, CancellationToken cancellationToken)
        {
            var gateway = await _context.PaymentGateways.FindAsync(request.Id);
            if (gateway == null) return Result<string>.Fail("Gateway not found.");

            var oldValues = JsonSerializer.Serialize(new
            {
                gateway.Name, gateway.GatewayFamily, gateway.Slug, gateway.Type, gateway.IsActive,
                gateway.IsSandbox, gateway.SupportedCurrencies, gateway.SortOrder
            });

            _context.PaymentGateways.Remove(gateway);
            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("PaymentGateway", "Delete",
                _userContextService.UserId, _userContextService.Email,
                $"Deleted gateway '{gateway.Name}'",
                entityId: request.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues);

            return Result<string>.Ok(null, "Gateway deleted.");
        }
    }
}
