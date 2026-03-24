using CarShop.Application.DTOs.Brand;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;

        private const string AllBrandsKey = "brands:all";
        private static string BrandKey(int id) => $"brands:{id}";

        public BrandService(
            IUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<IEnumerable<BrandDto>>> GetAllBrandsAsync()
        {
            var redis = await _unitOfWork.Repository<IntegrationSetting>().
                FirstOrDefaultAsync(s => s.ServiceName == "Redis", 
                s => new
                {
                    s.IsEnabled
                });
            var isRedisEnabled = redis != null && redis.IsEnabled;
            if (isRedisEnabled)
            {
                var cached = await _cacheService.GetAsync<IEnumerable<BrandDto>>(AllBrandsKey);
                if (cached != null)
                    return Result<IEnumerable<BrandDto>>.Ok(cached);
            }

            var brands = await _unitOfWork.Repository<Brand>().GetAllAsync();
            var result = brands.Select(b => new BrandDto { Id = b.Id, Name = b.Name });
            if (isRedisEnabled)
                await _cacheService.SetAsync(AllBrandsKey, result, TimeSpan.FromDays(1));
            return Result<IEnumerable<BrandDto>>.Ok(result);
        }

        public async Task<Result<BrandDto>> GetBrandByIdAsync(int id)
        {
            var redis = await _unitOfWork.Repository<IntegrationSetting>().
                FirstOrDefaultAsync(s => s.ServiceName == "Redis",
                s => new
                {
                    s.IsEnabled
                });
            var isRedisEnabled = redis != null && redis.IsEnabled;
            if (isRedisEnabled)
            {
                var cached = await _cacheService.GetAsync<BrandDto>(BrandKey(id));
                if (cached != null)
                    return Result<BrandDto>.Ok(cached);
            }

            var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(id);
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

            var brand = await _unitOfWork.Repository<Brand>().FirstOrDefaultAsync(b => b.Name == brandName);
            if (brand == null)
                return Result<int?>.Fail("Brand not found");

            return Result<int?>.Ok(brand.Id);
        }

        public async Task<Result<BrandDto?>> GetBrandByNameAsync(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                return Result<BrandDto?>.Fail("Brand name is required");

            var brand = await _unitOfWork.Repository<Brand>().FirstOrDefaultAsync(b => b.Name == brandName);
            if (brand == null)
                return Result<BrandDto?>.Fail("Brand not found");

            var dto = new BrandDto { Id = brand.Id, Name = brand.Name };
            return Result<BrandDto?>.Ok(dto);
        }

        public async Task<Result<int>> CreateBrandAsync(BrandDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<int>.Fail("Brand name is required");

            //var exists = await _brandRepository.ExistsByNameAsync(dto.Name);
            var exists = await _unitOfWork.Repository<Brand>().AnyAsync(b => b.Name == dto.Name);
            if (exists)
                return Result<int>.Fail("A brand with this name already exists.");

            var brand = new Brand { Name = dto.Name.Trim() };            
            await _unitOfWork.Repository<Brand>().AddAsync(brand);
            await _unitOfWork.SaveChangesAsync();
            var redis = await _unitOfWork.Repository<IntegrationSetting>().
                FirstOrDefaultAsync(s => s.ServiceName == "Redis",
                s => new
                {
                    s.IsEnabled
                });
            var isRedisEnabled = redis != null && redis.IsEnabled;
            // Invalidate cache
            if (isRedisEnabled)
                await _cacheService.RemoveAsync(AllBrandsKey);
            return Result<int>.Ok(brand.Id, "Brand created successfully.");
        }

        public async Task<Result<string>> UpdateBrandAsync(int id, BrandDto dto)
        {
            var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(id);
            if (brand == null)
                return Result<string>.Fail("Brand not found.");

            var exists = await _unitOfWork.Repository<Brand>().AnyAsync(b => b.Name == dto.Name && b.Id != id);
            if (exists)
                return Result<string>.Fail("Another brand with this name already exists.");

            brand.Name = dto.Name!.Trim();
            _unitOfWork.Repository<Brand>().Update(brand);
            await _unitOfWork.SaveChangesAsync();
            var redis = await _unitOfWork.Repository<IntegrationSetting>().
                FirstOrDefaultAsync(s => s.ServiceName == "Redis",
                s => new
                {
                    s.IsEnabled
                });
            var isRedisEnabled = redis != null && redis.IsEnabled;
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
            var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(id);
            if (brand == null)
                return Result<string>.Fail("Brand not found.");


            _unitOfWork.Repository<Brand>().Remove(brand);
            await _unitOfWork.SaveChangesAsync();
            var redis = await _unitOfWork.Repository<IntegrationSetting>().
                FirstOrDefaultAsync(s => s.ServiceName == "Redis",
                s => new
                {
                    s.IsEnabled
                });
            var isRedisEnabled = redis != null && redis.IsEnabled;
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
