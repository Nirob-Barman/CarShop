using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Wishlist.Queries.IsInWishlist
{
    public class IsInWishlistQueryHandler : IRequestHandler<IsInWishlistQuery, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public IsInWishlistQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<bool>> Handle(IsInWishlistQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var exists = await _context.WishlistItems.AsNoTracking().AnyAsync(w => w.UserId == userId && w.CarId == request.CarId);
            return Result<bool>.Ok(exists);
        }
    }
}
