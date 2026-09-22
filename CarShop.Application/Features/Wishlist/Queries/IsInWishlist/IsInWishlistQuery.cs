using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Queries.IsInWishlist
{
    public record IsInWishlistQuery(int CarId) : IRequest<Result<bool>>;
}
