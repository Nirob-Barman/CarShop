using CarShop.Application.DTOs.Brand;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Repositories;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;

        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<Result<IEnumerable<BrandDto>>> GetAllBrandsAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            var result = brands.Select(b => new BrandDto { Id = b.Id, Name = b.Name });
            return Result<IEnumerable<BrandDto>>.Ok(result);
        }

        public async Task<Result<BrandDto>> GetBrandByIdAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                return Result<BrandDto>.Fail("Brand not found");

            var dto = new BrandDto { Id = brand.Id, Name = brand.Name };
            return Result<BrandDto>.Ok(dto);
        }

        public async Task<Result<int?>> GetBrandIdByNameAsync(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                return Result<int?>.Fail("Brand name is required");

            var brandId = await _brandRepository.GetIdByNameAsync(brandName);
            if (brandId == null)
                return Result<int?>.Fail("Brand not found");

            return Result<int?>.Ok(brandId);
        }

        public async Task<Result<BrandDto?>> GetBrandByNameAsync(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                return Result<BrandDto?>.Fail("Brand name is required");

            var brand = await _brandRepository.GetByNameAsync(brandName);
            if (brand == null)
                return Result<BrandDto?>.Fail("Brand not found");

            var dto = new BrandDto { Id = brand.Id, Name = brand.Name };
            return Result<BrandDto?>.Ok(dto);
        }

        public async Task<Result<int>> CreateBrandAsync(BrandDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<int>.Fail("Brand name is required");

            var exists = await _brandRepository.ExistsByNameAsync(dto.Name);
            if (exists)
                return Result<int>.Fail("A brand with this name already exists.");

            var brand = new Brand { Name = dto.Name.Trim() };
            _brandRepository.AddAsync(brand);
            await _brandRepository.SaveChangesAsync();

            return Result<int>.Ok(brand.Id, "Brand created successfully.");
        }

        public async Task<Result<string>> UpdateBrandAsync(int id, BrandDto dto)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                return Result<string>.Fail("Brand not found.");

            var exists = await _brandRepository.ExistsByNameAsync(dto.Name!, excludeId: id);
            if (exists)
                return Result<string>.Fail("Another brand with this name already exists.");

            brand.Name = dto.Name!.Trim();
            _brandRepository.UpdateAsync(brand);
            await _brandRepository.SaveChangesAsync();

            return Result<string>.Ok(null, "Brand updated successfully.");
        }

        public async Task<Result<string>> DeleteBrandAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                return Result<string>.Fail("Brand not found.");

            _brandRepository.DeleteAsync(brand);
            await _brandRepository.SaveChangesAsync();

            return Result<string>.Ok(null, "Brand deleted successfully.");
        }
    }
}
