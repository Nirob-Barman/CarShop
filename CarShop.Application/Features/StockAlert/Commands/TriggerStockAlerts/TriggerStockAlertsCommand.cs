using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.StockAlert.Commands.TriggerStockAlerts
{
    public record TriggerStockAlertsCommand(int CarId)
        : IRequest<Result<string>>;
}
