using CarShop.Application.DTOs.Wishlist;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Wishlist.Queries.GetWishlist
{
    public class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, Result<IEnumerable<WishlistItemDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetWishlistQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<WishlistItemDto>>> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var items = await _context.WishlistItems.AsNoTracking()
                .Where(w => w.UserId == userId)
                .Select(w => new WishlistItemDto
                {
                    Id = w.Id,
                    CarId = w.CarId,
                    CarTitle = w.Car != null ? w.Car.Title : null,
                    CarPrice = w.Car != null ? w.Car.Price : 0,
                    CarImageUrl = w.Car != null ? w.Car.ImageUrl : null,
                    BrandName = w.Car != null && w.Car.Brand != null
                        ? w.Car.Brand.Name
                        : null,
                    AddedAt = w.AddedAt
                }).ToListAsync(cancellationToken);

            return Result<IEnumerable<WishlistItemDto>>.Ok(items);
        }
    }
}
