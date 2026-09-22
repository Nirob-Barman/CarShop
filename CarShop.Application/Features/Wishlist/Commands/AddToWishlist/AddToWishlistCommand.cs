using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Commands.AddToWishlist
{
    public record AddToWishlistCommand(int CarId)
        : IRequest<Result<string>>;
}
