using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.StockAlert.Commands.UnsubscribeStockAlert
{
    public record UnsubscribeStockAlertCommand(int CarId)
        : IRequest<Result<string>>;
}
