using CarShop.Application.Interfaces;
using MediatR;

namespace CarShop.Application.Features.AuditLog.Queries.GetDistinctAuditLogEntityNames
{
    public class GetDistinctAuditLogEntityNamesQueryHandler : IRequestHandler<GetDistinctAuditLogEntityNamesQuery, IEnumerable<string>>
    {
        private readonly IAuditLogService _auditLogService;

        public GetDistinctAuditLogEntityNamesQueryHandler(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<IEnumerable<string>> Handle(GetDistinctAuditLogEntityNamesQuery request, CancellationToken cancellationToken)
        {
            return await _auditLogService.GetDistinctEntityNamesAsync();
        }
    }
}
