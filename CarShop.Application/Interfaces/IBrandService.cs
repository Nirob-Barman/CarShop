using CarShop.Application.DTOs.Brand;

namespace CarShop.Application.Interfaces
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandDto>> GetAllBrandsAsync();
        Task<BrandDto> GetBrandByIdAsync(int id);
        Task<int> CreateBrandAsync(BrandDto dto);
        Task UpdateBrandAsync(int id, BrandDto dto);
        Task DeleteBrandAsync(int id);
    }
}
