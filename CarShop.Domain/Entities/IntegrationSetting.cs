using CarShop.Domain.Enums;

namespace CarShop.Domain.Entities
{
    public class IntegrationSetting : BaseAuditableEntity
    {
        public string? ServiceName { get; set; }
        public bool IsEnabled { get; set; }
        // Optional JSON or string config for the service
        public string? Configuration { get; set; }
        // Optional: TTL in seconds for caching services
        public int? CacheTTLSeconds { get; set; }
        // Optional: service priority
        public int? Priority { get; set; }
        // Optional: retry policy configuration
        public string? RetryPolicy { get; set; }
        // Optional: last time the service was used
        public DateTime? LastUsedAt { get; set; }
        // Optional: service status
        public ServiceStatus Status { get; set; } = ServiceStatus.Inactive;
    }
}
