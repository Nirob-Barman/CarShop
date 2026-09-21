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
                .Include(w => w.Car)
                .Where(w => w.UserId == userId)
                .ToListAsync(cancellationToken);

            var dtos = items.Select(w => new WishlistItemDto
            {
                Id = w.Id,
                CarId = w.CarId,
                CarTitle = w.Car?.Title,
                CarPrice = w.Car?.Price ?? 0,
                CarImageUrl = w.Car?.ImageUrl,
                BrandName = w.Car?.Brand?.Name,
                AddedAt = w.AddedAt
            });

            return Result<IEnumerable<WishlistItemDto>>.Ok(dtos);
        }
    }
}
