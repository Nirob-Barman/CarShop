using System.Text.Json;
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
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        private const string AllBrandsKey = "brands:all";
        private static string BrandKey(int id) => $"brands:{id}";

        public BrandService(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
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

            await _auditLogService.LogAsync("Brand", "Create", _userContextService.UserId, _userContextService.Email,
                $"Created brand: {brand.Name} (Id: {brand.Id})",
                entityId: brand.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                newValues: JsonSerializer.Serialize(new { brand.Id, brand.Name }));

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

            var oldValues = JsonSerializer.Serialize(new { brand.Id, brand.Name });
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

            await _auditLogService.LogAsync("Brand", "Update", _userContextService.UserId, _userContextService.Email,
                $"Updated brand: {brand.Name} (Id: {id})",
                entityId: id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues,
                newValues: JsonSerializer.Serialize(new { brand.Id, brand.Name }));

            return Result<string>.Ok(null, "Brand updated successfully.");
        }

        public async Task<Result<string>> DeleteBrandAsync(int id)
        {
            var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(id);
            if (brand == null)
                return Result<string>.Fail("Brand not found.");

            var oldValues = JsonSerializer.Serialize(new { brand.Id, brand.Name });
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

            await _auditLogService.LogAsync("Brand", "Delete", _userContextService.UserId, _userContextService.Email,
                $"Deleted brand: {brand.Name} (Id: {id})",
                entityId: id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues);

            return Result<string>.Ok(null, "Brand deleted successfully.");
        }
    }
}
