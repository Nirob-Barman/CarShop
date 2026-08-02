using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.CreatePendingOrder
{
    public class CreatePendingOrderCommand : IRequest<Result<(int OrderId, decimal FinalPrice, string CarTitle)>>
    {
        public int CarId { get; set; }
        public string? PromoCode { get; set; }

        public CreatePendingOrderCommand(int carId, string? promoCode = null)
        {
            CarId = carId;
            PromoCode = promoCode;
        }
    }
}
