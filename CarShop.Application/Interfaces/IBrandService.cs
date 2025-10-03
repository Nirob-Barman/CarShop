using CarShop.Application.DTOs.Brand;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface IBrandService
    {
        Task<Result<IEnumerable<BrandDto>>> GetAllBrandsAsync();
        Task<Result<BrandDto>> GetBrandByIdAsync(int id);
        Task<Result<int?>> GetBrandIdByNameAsync(string brandName);
        Task<Result<BrandDto?>> GetBrandByNameAsync(string brandName);
        Task<Result<int>> CreateBrandAsync(BrandDto dto);
        Task<Result<string>> UpdateBrandAsync(int id, BrandDto dto);
        Task<Result<string>> DeleteBrandAsync(int id);

        //Task<IEnumerable<BrandDto>> GetAllBrandsAsync();
        //Task<BrandDto> GetBrandByIdAsync(int id);
        //Task<int?> GetBrandIdByNameAsync(string brandName);
        //Task<BrandDto?> GetBrandByNameAsync(string brandName);
        //Task<int> CreateBrandAsync(BrandDto dto);
        //Task UpdateBrandAsync(int id, BrandDto dto);
        //Task DeleteBrandAsync(int id);
    }
}
