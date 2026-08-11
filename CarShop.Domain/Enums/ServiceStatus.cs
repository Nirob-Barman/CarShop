namespace CarShop.Domain.Enums
{
    public enum ServiceStatus
    {
        Active,   // Service is enabled and can be used
        Inactive, // Service is intentionally turned off
        Paused,   // Temporarily unavailable, maybe for maintenance
        Error     // Service has failed or is unreliable
    }
}
