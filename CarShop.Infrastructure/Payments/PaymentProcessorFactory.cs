using CarShop.Application.Interfaces;

namespace CarShop.Infrastructure.Payments
{
    public class PaymentProcessorFactory : IPaymentProcessorFactory
    {
        private readonly Dictionary<string, IPaymentProcessor> _processors;

        public PaymentProcessorFactory(IEnumerable<IPaymentProcessor> processors)
        {
            _processors = processors.ToDictionary(p => p.GatewaySlug, StringComparer.OrdinalIgnoreCase);
        }

        public IPaymentProcessor GetProcessor(string slug)
        {
            if (_processors.TryGetValue(slug, out var processor)) return processor;
            throw new InvalidOperationException($"No payment processor registered for gateway slug '{slug}'.");
        }

        public bool HasProcessor(string slug) => _processors.ContainsKey(slug);
    }
}
