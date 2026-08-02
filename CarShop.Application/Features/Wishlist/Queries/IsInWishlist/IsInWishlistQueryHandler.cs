using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Queries.IsInWishlist
{
    public class IsInWishlistQueryHandler : IRequestHandler<IsInWishlistQuery, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public IsInWishlistQueryHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<bool>> Handle(IsInWishlistQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var exists = await _unitOfWork.Repository<WishlistItem>().AnyAsync(w => w.UserId == userId && w.CarId == request.CarId);
            return Result<bool>.Ok(exists);
        }
    }
}
