using CarShop.Application.DTOs.AuditLog;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.AuditLog.Queries.GetAuditLogs
{
    public class GetAuditLogsQuery : IRequest<Result<IEnumerable<AuditLogDto>>>
    {
        public string? EntityName { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        public GetAuditLogsQuery(string? entityName = null, int page = 1, int pageSize = 50)
        {
            EntityName = entityName;
            Page = page;
            PageSize = pageSize;
        }
    }
}
