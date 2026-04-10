using CarShop.Application.DTOs.AuditLog;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(
            string entityName,
            string action,
            string? userId,
            string? userName,
            string? details = null,
            int? entityId = null,
            string? ipAddress = null,
            string? userAgent = null,
            string? oldValues = null,
            string? newValues = null);

        Task<Result<IEnumerable<AuditLogDto>>> GetLogsAsync(string? entityName = null, int page = 1, int pageSize = 50);
        Task<IEnumerable<string>> GetDistinctEntityNamesAsync();
    }
}
