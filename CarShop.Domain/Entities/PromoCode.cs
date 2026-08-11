namespace CarShop.Domain.Entities
{
    public class PromoCode : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? MaxUsages { get; set; }
        public int UsageCount { get; private set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; set; }

        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
        public bool HasReachedUsageLimit => MaxUsages.HasValue && UsageCount >= MaxUsages.Value;

        public void RecordUsage() => UsageCount++;

        public void Deactivate() => IsActive = false;

        public void ToggleActive() => IsActive = !IsActive;
    }
}
