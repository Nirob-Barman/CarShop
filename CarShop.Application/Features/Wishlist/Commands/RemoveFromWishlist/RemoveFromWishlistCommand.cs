using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Commands.RemoveFromWishlist
{
    public record RemoveFromWishlistCommand(int CarId)
        : IRequest<Result<string>>;
}
