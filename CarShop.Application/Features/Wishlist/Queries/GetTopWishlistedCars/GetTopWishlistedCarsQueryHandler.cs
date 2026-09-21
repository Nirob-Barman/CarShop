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
            var allItems = await _context.WishlistItems
                .Include(w => w.Car)
                .ThenInclude(c => c!.Brand)
                .ToListAsync(cancellationToken);

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
