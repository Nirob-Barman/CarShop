using CarShop.Application.DTOs.Wishlist;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Queries.GetTopWishlistedCars
{
    public class GetTopWishlistedCarsQuery : IRequest<Result<IEnumerable<TopWishlistedCarDto>>>
    {
        public int Count { get; set; }

        public GetTopWishlistedCarsQuery(int count = 4)
        {
            Count = count;
        }
    }
}
