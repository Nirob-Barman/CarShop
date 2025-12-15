using CarShop.Domain.Entities;

namespace CarShop.Application.Interfaces.Repositories.Integration
{
    public interface IIntegrationRepository
    {
        Task<bool> IsEnabledAsync(string serviceName);
        Task<IntegrationSetting?> GetSettingAsync(string serviceName);
    }
}
