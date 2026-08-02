using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;

namespace CarShop.Application.Features.Wishlist.Commands.AddToWishlist
{
    public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public AddToWishlistCommandHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var exists = await _unitOfWork.Repository<WishlistItem>().AnyAsync(w => w.UserId == userId && w.CarId == request.CarId);
            if (exists)
                return Result<string>.Fail("Car is already in your wishlist.");

            var car = await _unitOfWork.Repository<CarEntity>().GetByIdAsync(request.CarId);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            var item = new WishlistItem
            {
                UserId = userId,
                CarId = request.CarId,
                AddedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<WishlistItem>().AddAsync(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Added to wishlist.");
        }
    }
}
