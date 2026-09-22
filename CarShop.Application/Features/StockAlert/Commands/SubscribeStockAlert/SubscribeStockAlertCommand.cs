using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.StockAlert.Commands.SubscribeStockAlert
{
    public record SubscribeStockAlertCommand(int CarId)
        : IRequest<Result<string>>;
}
