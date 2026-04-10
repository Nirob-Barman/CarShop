using CarShop.Application.DTOs.StockAlert;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface IStockAlertService
    {
        Task<Result<string>> SubscribeAsync(int carId);
        Task<Result<string>> UnsubscribeAsync(int carId);
        Task<Result<IEnumerable<StockAlertDto>>> GetUserAlertsAsync();
        Task TriggerAlertsForCarAsync(int carId);
    }
}
