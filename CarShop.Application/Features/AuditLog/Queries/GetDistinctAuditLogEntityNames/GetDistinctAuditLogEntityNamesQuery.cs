using MediatR;

namespace CarShop.Application.Features.AuditLog.Queries.GetDistinctAuditLogEntityNames
{
    public class GetDistinctAuditLogEntityNamesQuery : IRequest<IEnumerable<string>>
    {
    }
}
