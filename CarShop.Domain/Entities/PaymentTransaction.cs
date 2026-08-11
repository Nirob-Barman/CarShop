namespace CarShop.Domain.Entities
{
    public class PaymentTransaction : BaseEntity
    {
        public int OrderId { get; set; }
        public int PaymentGatewayId { get; set; }
        public string? SessionRef { get; set; }
        public string? TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public PaymentTransactionStatus Status { get; private set; } = PaymentTransactionStatus.Pending;
        public string? RawResponse { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order? Order { get; set; }
        public PaymentGateway? PaymentGateway { get; set; }

        public void MarkFailed() => Status = PaymentTransactionStatus.Failed;

        public void RecordVerificationResult(bool success, string? providerTransactionId, string? rawResponse)
        {
            Status = success ? PaymentTransactionStatus.Success : PaymentTransactionStatus.Failed;
            TransactionId = providerTransactionId;
            RawResponse = rawResponse;
        }
    }
}
