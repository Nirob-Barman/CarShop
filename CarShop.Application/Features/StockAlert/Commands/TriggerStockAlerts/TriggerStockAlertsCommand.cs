using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.StockAlert.Commands.TriggerStockAlerts
{
    public class TriggerStockAlertsCommand : IRequest<Result<string>>
    {
        public int CarId { get; set; }

        public TriggerStockAlertsCommand(int carId)
        {
            CarId = carId;
        }
    }
}
