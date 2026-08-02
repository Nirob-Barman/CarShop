using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Commands.RemoveFromWishlist
{
    public class RemoveFromWishlistCommandHandler : IRequestHandler<RemoveFromWishlistCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public RemoveFromWishlistCommandHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var item = await _unitOfWork.Repository<WishlistItem>().FirstOrDefaultAsync(w => w.UserId == userId && w.CarId == request.CarId);
            if (item == null)
                return Result<string>.Fail("Item not found in wishlist.");

            _unitOfWork.Repository<WishlistItem>().Remove(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Removed from wishlist.");
        }
    }
}
