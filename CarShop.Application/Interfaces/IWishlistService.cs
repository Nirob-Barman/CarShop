using CarShop.Application.DTOs.Wishlist;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface IWishlistService
    {
        Task<Result<string>> AddToWishlistAsync(int carId);
        Task<Result<string>> RemoveFromWishlistAsync(int carId);
        Task<Result<IEnumerable<WishlistItemDto>>> GetWishlistAsync();
        Task<Result<bool>> IsInWishlistAsync(int carId);
        Task<Result<IEnumerable<TopWishlistedCarDto>>> GetTopWishlistedCarsAsync(int count = 4);
    }
}
