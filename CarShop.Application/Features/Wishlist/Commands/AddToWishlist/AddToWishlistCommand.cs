using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Commands.AddToWishlist
{
    public class AddToWishlistCommand : IRequest<Result<string>>
    {
        public int CarId { get; set; }

        public AddToWishlistCommand(int carId)
        {
            CarId = carId;
        }
    }
}
