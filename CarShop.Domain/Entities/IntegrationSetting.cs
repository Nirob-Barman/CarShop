namespace CarShop.Domain.Entities
{
    public class IntegrationSetting
    {
        public int Id { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public enum ServiceStatus
    {
        Active,   // Service is enabled and can be used
        Inactive, // Service is intentionally turned off
        Paused,   // Temporarily unavailable, maybe for maintenance
        Error     // Service has failed or is unreliable
    }
}
