namespace CarShop.Domain.Entities
{
    public class PromoCode : BaseEntity
    {
        public string Code { get; private set; } = string.Empty;
        public decimal DiscountPercent { get; private set; }
        public decimal? MaxDiscountAmount { get; private set; }
        public int? MaxUsages { get; private set; }
        public int UsageCount { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }

        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
        public bool HasReachedUsageLimit => MaxUsages.HasValue && UsageCount >= MaxUsages.Value;
        public bool IsUsable => IsActive && !IsExpired && !HasReachedUsageLimit;
        private PromoCode(
            string code,
            decimal discountPercent,
            decimal? maxDiscountAmount,
            int? maxUsages,
            DateTime? expiresAt)
        {
            SetCode(code);
            ValidateDiscountPercent(discountPercent);
            ValidateMaxDiscountAmount(maxDiscountAmount);
            ValidateMaxUsages(maxUsages);

            DiscountPercent = discountPercent;
            MaxDiscountAmount = maxDiscountAmount;
            MaxUsages = maxUsages;

            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            UsageCount = 0;
        }

        public static PromoCode Create(
            string code,
            decimal discountPercent,
            decimal? maxDiscountAmount = null,
            int? maxUsages = null,
            DateTime? expiresAt = null)
        {
            return new PromoCode(
                code,
                discountPercent,
                maxDiscountAmount,
                maxUsages,
                expiresAt);
        }

        public void Update(
            decimal discountPercent,
            decimal? maxDiscountAmount,
            int? maxUsages,
            DateTime? expiresAt)
        {
            ValidateDiscountPercent(discountPercent);
            ValidateMaxDiscountAmount(maxDiscountAmount);
            ValidateMaxUsages(maxUsages);

            if (maxUsages.HasValue && maxUsages.Value < UsageCount)
            {
                throw new InvalidOperationException(
                    "Maximum usages cannot be less than the current usage count.");
            }

            DiscountPercent = discountPercent;
            MaxDiscountAmount = maxDiscountAmount;
            MaxUsages = maxUsages;
            ExpiresAt = expiresAt;
        }


        public void RecordUsage()
        {
            if (!IsActive)
                throw new InvalidOperationException("Promo code is inactive.");

            if (IsExpired)
                throw new InvalidOperationException("Promo code has expired.");

            if (HasReachedUsageLimit)
                throw new InvalidOperationException("Promo code usage limit has been reached.");

            UsageCount++;
        }

        public void Activate()
        {
            if (IsExpired)
                throw new InvalidOperationException(
                    "An expired promo code cannot be activated.");

            IsActive = true;
        }

        public void Deactivate() => IsActive = false;

        //public void ToggleActive() => IsActive = !IsActive;

        public void ToggleActive()
        {
            if (IsActive)
                Deactivate();
            else
                Activate();
        }

        private void SetCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Promo code is required.", nameof(code));

            Code = code.Trim().ToUpperInvariant();
        }

        private static void ValidateDiscountPercent(decimal value)
        {
            if (value <= 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Discount percent must be greater than 0 and less than or equal to 100.");
        }

        private static void ValidateMaxDiscountAmount(decimal? value)
        {
            if (value.HasValue && value.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Maximum discount amount must be greater than 0.");
        }

        private static void ValidateMaxUsages(int? value)
        {
            if (value.HasValue && value.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Maximum usages must be greater than 0.");
        }
    }
}
