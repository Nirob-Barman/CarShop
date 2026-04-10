namespace CarShop.Application.Interfaces
{
    public interface IPaymentProcessorFactory
    {
        IPaymentProcessor GetProcessor(string slug);
        bool HasProcessor(string slug);
    }
}
