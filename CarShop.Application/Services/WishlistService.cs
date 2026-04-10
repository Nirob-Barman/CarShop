using CarShop.Application.DTOs.Wishlist;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public WishlistService(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> AddToWishlistAsync(int carId)
        {
            var userId = _userContextService.UserId!;
            var exists = await _unitOfWork.Repository<WishlistItem>().AnyAsync(w => w.UserId == userId && w.CarId == carId);
            if (exists)
                return Result<string>.Fail("Car is already in your wishlist.");

            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(carId);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            var item = new WishlistItem
            {
                UserId = userId,
                CarId = carId,
                AddedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<WishlistItem>().AddAsync(item);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "Added to wishlist.");
        }

        public async Task<Result<string>> RemoveFromWishlistAsync(int carId)
        {
            var userId = _userContextService.UserId!;
            var item = await _unitOfWork.Repository<WishlistItem>().FirstOrDefaultAsync(w => w.UserId == userId && w.CarId == carId);
            if (item == null)
                return Result<string>.Fail("Item not found in wishlist.");

            _unitOfWork.Repository<WishlistItem>().Remove(item);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "Removed from wishlist.");
        }

        public async Task<Result<IEnumerable<WishlistItemDto>>> GetWishlistAsync()
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

        public async Task<Result<bool>> IsInWishlistAsync(int carId)
        {
            var userId = _userContextService.UserId!;
            var exists = await _unitOfWork.Repository<WishlistItem>().AnyAsync(w => w.UserId == userId && w.CarId == carId);
            return Result<bool>.Ok(exists);
        }

        public async Task<Result<IEnumerable<TopWishlistedCarDto>>> GetTopWishlistedCarsAsync(int count = 4)
        {
            var allItems = await _unitOfWork.Repository<WishlistItem>().GetAllWithIncludesAsync(
                predicate: _ => true,
                selector:  w => w,
                w => w.Car!,
                w => w.Car!.Brand!);

            var top = allItems
                .GroupBy(w => w.CarId)
                .Select(g => new TopWishlistedCarDto
                {
                    CarId         = g.Key,
                    CarTitle      = g.First().Car?.Title,
                    CarPrice      = g.First().Car?.Price ?? 0,
                    CarImageUrl   = g.First().Car?.ImageUrl,
                    BrandName     = g.First().Car?.Brand?.Name,
                    WishlistCount = g.Count()
                })
                .OrderByDescending(x => x.WishlistCount)
                .Take(count);

            return Result<IEnumerable<TopWishlistedCarDto>>.Ok(top);
        }
    }
}
