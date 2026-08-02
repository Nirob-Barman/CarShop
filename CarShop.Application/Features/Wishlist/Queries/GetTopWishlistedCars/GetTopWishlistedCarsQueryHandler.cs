using CarShop.Application.DTOs.Wishlist;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Queries.GetTopWishlistedCars
{
    public class GetTopWishlistedCarsQueryHandler : IRequestHandler<GetTopWishlistedCarsQuery, Result<IEnumerable<TopWishlistedCarDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTopWishlistedCarsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<TopWishlistedCarDto>>> Handle(GetTopWishlistedCarsQuery request, CancellationToken cancellationToken)
        {
            var allItems = await _unitOfWork.Repository<WishlistItem>().GetAllWithIncludesAsync(
                predicate: _ => true,
                selector: w => w,
                w => w.Car!,
                w => w.Car!.Brand!);

            var top = allItems
                .GroupBy(w => w.CarId)
                .Select(g => new TopWishlistedCarDto
                {
                    CarId = g.Key,
                    CarTitle = g.First().Car?.Title,
                    CarPrice = g.First().Car?.Price ?? 0,
                    CarImageUrl = g.First().Car?.ImageUrl,
                    BrandName = g.First().Car?.Brand?.Name,
                    WishlistCount = g.Count()
                })
                .OrderByDescending(x => x.WishlistCount)
                .Take(request.Count);

            return Result<IEnumerable<TopWishlistedCarDto>>.Ok(top);
        }
    }
}
