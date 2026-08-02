using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.StockAlert.Commands.SubscribeStockAlert
{
    public class SubscribeStockAlertCommand : IRequest<Result<string>>
    {
        public int CarId { get; set; }

        public SubscribeStockAlertCommand(int carId)
        {
            CarId = carId;
        }
    }
}
