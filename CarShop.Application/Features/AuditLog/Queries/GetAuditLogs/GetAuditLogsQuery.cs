using CarShop.Application.DTOs.AuditLog;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.AuditLog.Queries.GetAuditLogs
{
    public record GetAuditLogsQuery(
        string? EntityName = null,
        int Page = 1,
        int PageSize = 50
    ) : IRequest<Result<IEnumerable<AuditLogDto>>>;
}
