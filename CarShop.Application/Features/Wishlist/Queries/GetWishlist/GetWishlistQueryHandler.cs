using CarShop.Application.DTOs.Wishlist;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;

namespace CarShop.Application.Features.Wishlist.Queries.GetWishlist
{
    public class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, Result<IEnumerable<WishlistItemDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public GetWishlistQueryHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<WishlistItemDto>>> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var items = await _unitOfWork.Repository<WishlistItem>().GetAllWithIncludesAsync(
                predicate: w => w.UserId == userId,
                selector: w => w,
                w => w.Car!
            );

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
