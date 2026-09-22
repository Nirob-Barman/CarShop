using CarShop.Application.DTOs.Wishlist;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Queries.GetTopWishlistedCars
{
    public record GetTopWishlistedCarsQuery(int Count = 4)
        : IRequest<Result<IEnumerable<TopWishlistedCarDto>>>;
}
