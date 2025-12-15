using CarShop.Application.Interfaces.Repositories.Integration;
using CarShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Persistence.Repositories
{
    public class IntegrationRepository : IIntegrationRepository
    {
        private readonly AppDbContext _context;
        public IntegrationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsEnabledAsync(string serviceName)
        {
            var setting = await _context.IntegrationSettings.FirstOrDefaultAsync(s => s.ServiceName == serviceName);
            return setting?.IsEnabled ?? false;
        }

        public async Task<IntegrationSetting?> GetSettingAsync(string serviceName)
        {
            return await _context.IntegrationSettings.FirstOrDefaultAsync(s => s.ServiceName == serviceName);
        }
    }
}
