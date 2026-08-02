using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Queries.IsInWishlist
{
    public class IsInWishlistQuery : IRequest<Result<bool>>
    {
        public int CarId { get; set; }

        public IsInWishlistQuery(int carId)
        {
            CarId = carId;
        }
    }
}
