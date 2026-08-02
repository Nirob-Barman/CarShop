using CarShop.Application.DTOs.StockAlert;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.StockAlert.Queries.GetUserAlerts
{
    public class GetUserAlertsQuery : IRequest<Result<IEnumerable<StockAlertDto>>>
    {
    }
}
