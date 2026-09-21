using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Wishlist.Commands.RemoveFromWishlist
{
    public class RemoveFromWishlistCommandHandler : IRequestHandler<RemoveFromWishlistCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public RemoveFromWishlistCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var item = await _context.WishlistItems.FirstOrDefaultAsync(w => w.UserId == userId && w.CarId == request.CarId);
            if (item == null)
                return Result<string>.Fail("Item not found in wishlist.");

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Removed from wishlist.");
        }
    }
}
