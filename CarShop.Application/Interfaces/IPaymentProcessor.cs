namespace CarShop.Application.Interfaces
{
    public record PaymentRequest(
        int OrderId,
        int TransactionDbId,
        string CarTitle,
        decimal Amount,
        string Currency,
        string SuccessUrl,
        string CancelUrl,
        Dictionary<string, string> Config);

    public record PaymentInitResult(bool Success, string? RedirectUrl, string? SessionRef, string? Error);
    public record PaymentVerifyResult(bool Success, string? ProviderTransactionId, string? RawResponse, string? Error);

    public interface IPaymentProcessor
    {
        string GatewaySlug { get; }
        Task<PaymentInitResult> InitiateAsync(PaymentRequest request);
        Task<PaymentVerifyResult> VerifyAsync(string sessionRef, Dictionary<string, string> config);
    }
}
