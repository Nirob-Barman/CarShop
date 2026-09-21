using CarShop.Application.DTOs.AuditLog;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IApplicationDbContext _context;

        public AuditLogService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            string entityName,
            string action,
            string? userId,
            string? userName,
            string? details = null,
            int? entityId = null,
            string? ipAddress = null,
            string? userAgent = null,
            string? oldValues = null,
            string? newValues = null)
        {
            var log = new AuditLog
            {
                EntityName = entityName,
                Action = action,
                UserId = userId,
                UserName = userName,
                Details = details,
                EntityId = entityId,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                OldValues = oldValues,
                NewValues = newValues,
                Timestamp = DateTime.UtcNow
            };

            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<Result<IEnumerable<AuditLogDto>>> GetLogsAsync(string? entityName = null, int page = 1, int pageSize = 50)
        {
            var logs = await _context.AuditLogs.Where(l => entityName == null || l.EntityName == entityName).Select(l => new AuditLogDto
                {
                    Id = l.Id,
                    EntityId = l.EntityId,
                    EntityName = l.EntityName,
                    Action = l.Action,
                    UserId = l.UserId,
                    UserName = l.UserName,
                    IPAddress = l.IPAddress,
                    UserAgent = l.UserAgent,
                    Details = l.Details,
                    OldValues = l.OldValues,
                    NewValues = l.NewValues,
                    Timestamp = l.Timestamp
                }).ToListAsync();

            var paged = logs.OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return Result<IEnumerable<AuditLogDto>>.Ok(paged);
        }

        public async Task<IEnumerable<string>> GetDistinctEntityNamesAsync()
        {
            var names = await _context.AuditLogs
                .Select(l => l.EntityName!)
                .ToListAsync();
            return names.Where(n => !string.IsNullOrEmpty(n)).Distinct().OrderBy(n => n);
        }
    }
}
