using CarShop.Application.DTOs.AuditLog;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.AuditLog.Queries.GetAuditLogs
{
    public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, Result<IEnumerable<AuditLogDto>>>
    {
        private readonly IAuditLogService _auditLogService;

        public GetAuditLogsQueryHandler(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<Result<IEnumerable<AuditLogDto>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
        {
            return await _auditLogService.GetLogsAsync(request.EntityName, request.Page, request.PageSize);
        }
    }
}
