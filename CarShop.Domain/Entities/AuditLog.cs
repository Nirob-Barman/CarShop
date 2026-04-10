namespace CarShop.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int? EntityId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Details { get; set; }
        /// <summary>
        /// JSON serialized snapshot of the entity before the change.
        /// </summary>
        public string? OldValues { get; set; }
        /// <summary>
        /// JSON serialized snapshot of the entity after the change.
        /// </summary>
        public string? NewValues { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
