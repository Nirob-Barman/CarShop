using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.StockAlert.Commands.UnsubscribeStockAlert
{
    public class UnsubscribeStockAlertCommand : IRequest<Result<string>>
    {
        public int CarId { get; set; }

        public UnsubscribeStockAlertCommand(int carId)
        {
            CarId = carId;
        }
    }
}
