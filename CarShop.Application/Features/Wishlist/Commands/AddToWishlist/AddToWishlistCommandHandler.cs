using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Wishlist.Commands.AddToWishlist
{
    public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public AddToWishlistCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var exists = await _context.WishlistItems.AnyAsync(w => w.UserId == userId && w.CarId == request.CarId);
            if (exists)
                return Result<string>.Fail("Car is already in your wishlist.");

            var car = await _context.Cars.FindAsync(request.CarId);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            var item = new WishlistItem
            {
                UserId = userId,
                CarId = request.CarId,
                AddedAt = DateTime.UtcNow
            };

            await _context.WishlistItems.AddAsync(item);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Added to wishlist.");
        }
    }
}
