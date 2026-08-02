using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Commands.RemoveFromWishlist
{
    public class RemoveFromWishlistCommand : IRequest<Result<string>>
    {
        public int CarId { get; set; }

        public RemoveFromWishlistCommand(int carId)
        {
            CarId = carId;
        }
    }
}
