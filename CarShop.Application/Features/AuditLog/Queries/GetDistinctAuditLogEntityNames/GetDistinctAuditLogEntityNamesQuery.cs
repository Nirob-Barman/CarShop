using MediatR;

namespace CarShop.Application.Features.AuditLog.Queries.GetDistinctAuditLogEntityNames
{
    public record GetDistinctAuditLogEntityNamesQuery : IRequest<IEnumerable<string>>;
}
