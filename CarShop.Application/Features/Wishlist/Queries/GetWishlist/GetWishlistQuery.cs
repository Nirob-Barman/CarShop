using CarShop.Application.DTOs.Wishlist;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Queries.GetWishlist
{
    public record GetWishlistQuery : IRequest<Result<IEnumerable<WishlistItemDto>>>;
}
