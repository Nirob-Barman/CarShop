using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Wishlist;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Wishlist.Queries.GetTopWishlistedCars
{
    public class GetTopWishlistedCarsQueryHandler : IRequestHandler<GetTopWishlistedCarsQuery, Result<IEnumerable<TopWishlistedCarDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetTopWishlistedCarsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<TopWishlistedCarDto>>> Handle(GetTopWishlistedCarsQuery request, CancellationToken cancellationToken)
        {
            var top = await _context.WishlistItems.AsNoTracking()
                .GroupBy(w => new
                {
                    w.CarId,
                    w.Car!.Title,
                    w.Car.Price,
                    w.Car.ImageUrl,
                    BrandName = w.Car.Brand != null
                        ? w.Car.Brand.Name
                        : null
                })
                .Select(g => new TopWishlistedCarDto
                {
                    CarId = g.Key.CarId,
                    CarTitle = g.Key.Title,
                    CarPrice = g.Key.Price,
                    CarImageUrl = g.Key.ImageUrl,
                    BrandName = g.Key.BrandName,
                    WishlistCount = g.Count()
                })
                .OrderByDescending(x => x.WishlistCount)
                .Take(request.Count)
                .ToListAsync(cancellationToken);

            return Result<IEnumerable<TopWishlistedCarDto>>.Ok(top);
        }
    }
}
