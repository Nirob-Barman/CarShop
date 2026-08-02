using CarShop.Application.DTOs.Wishlist;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Queries.GetWishlist
{
    public class GetWishlistQuery : IRequest<Result<IEnumerable<WishlistItemDto>>>
    {
    }
}
