using CarShop.Application.DTOs.Brand;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Interfaces.Repositories;
using CarShop.Application.Interfaces.Repositories.Integration;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly ICacheService _cacheService;
        private readonly IIntegrationRepository _integrationRepository;

        private const string AllBrandsKey = "brands:all";
        private static string BrandKey(int id) => $"brands:{id}";

        public BrandService(IBrandRepository brandRepository, ICacheService cacheService, IIntegrationRepository integrationRepository)
        {
            _brandRepository = brandRepository;
            _cacheService = cacheService;
            _integrationRepository = integrationRepository;
        }

        public async Task<Result<IEnumerable<BrandDto>>> GetAllBrandsAsync()
        {
            var isRedisEnabled = await _integrationRepository.IsEnabledAsync("Redis");
            if (isRedisEnabled)
            {
                var cached = await _cacheService.GetAsync<IEnumerable<BrandDto>>(AllBrandsKey);
                if (cached != null)
                    return Result<IEnumerable<BrandDto>>.Ok(cached);
            }

            var brands = await _brandRepository.GetAllAsync();
            var result = brands.Select(b => new BrandDto { Id = b.Id, Name = b.Name });
            if (isRedisEnabled)
                await _cacheService.SetAsync(AllBrandsKey, result, TimeSpan.FromDays(1));
            return Result<IEnumerable<BrandDto>>.Ok(result);
        }

        public async Task<Result<BrandDto>> GetBrandByIdAsync(int id)
        {
            var isRedisEnabled = await _integrationRepository.IsEnabledAsync("Redis");
            if (isRedisEnabled)
            {
                var cached = await _cacheService.GetAsync<BrandDto>(BrandKey(id));
                if (cached != null)
                    return Result<BrandDto>.Ok(cached);
            }

            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                return Result<BrandDto>.Fail("Brand not found");

            var dto = new BrandDto { Id = brand.Id, Name = brand.Name };
            if (isRedisEnabled)
                await _cacheService.SetAsync(BrandKey(id), dto, TimeSpan.FromMinutes(10));
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
            var isRedisEnabled = await _integrationRepository.IsEnabledAsync("Redis");
            // Invalidate cache
            if (isRedisEnabled)
                await _cacheService.RemoveAsync(AllBrandsKey);
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
            var isRedisEnabled = await _integrationRepository.IsEnabledAsync("Redis");
            // Invalidate cache
            if (isRedisEnabled)
            {
                await _cacheService.RemoveAsync(AllBrandsKey);
                await _cacheService.RemoveAsync(BrandKey(id));
            }
            return Result<string>.Ok(null, "Brand updated successfully.");
        }

        public async Task<Result<string>> DeleteBrandAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                return Result<string>.Fail("Brand not found.");

            _brandRepository.DeleteAsync(brand);
            await _brandRepository.SaveChangesAsync();
            var isRedisEnabled = await _integrationRepository.IsEnabledAsync("Redis");
            // Invalidate cache
            if (isRedisEnabled)
            {
                await _cacheService.RemoveAsync(AllBrandsKey);
                await _cacheService.RemoveAsync(BrandKey(id));
            }

            return Result<string>.Ok(null, "Brand deleted successfully.");
        }
    }
}
