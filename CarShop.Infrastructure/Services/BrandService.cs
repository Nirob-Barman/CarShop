
using CarShop.Application.DTOs.Brand;
using CarShop.Application.Interfaces;
using CarShop.Domain.Entities;
using CarShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext _context;

        public BrandService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            return await _context.Brands
                .Select(b => new BrandDto { Id = b.Id, Name = b.Name }).ToListAsync();
        }

        public async Task<BrandDto> GetBrandByIdAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            return brand != null ? new BrandDto { Id = brand.Id, Name = brand.Name } : null!;
        }

        public async Task<int?> GetBrandIdByNameAsync(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                return null;

            var normalizedBrandName = brandName.Trim().ToLower();

            var brand = await _context.Brands
                .Where(b => b.Name!.ToLower() == normalizedBrandName)
                .Select(b => new { b.Id })
                .FirstOrDefaultAsync();

            return brand?.Id;
        }

        public async Task<BrandDto?> GetBrandByNameAsync(string brandName)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Name == brandName);
            
            if (brand == null) return null;

            return new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name
            };
        }

        public async Task<int> CreateBrandAsync(BrandDto dto)
        {
            // Check if brand name already exists (case-insensitive)
            var exists = await _context.Brands.AnyAsync(b => b.Name!.ToLower() == dto.Name!.Trim().ToLower());
            if (exists)
            {
                throw new Exception("A brand with this name already exists.");
            }
            var brand = new Brand { Name = dto.Name!.Trim() };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return brand.Id;
        }

        public async Task UpdateBrandAsync(int id, BrandDto dto)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                throw new Exception("Brand not found.");
            }

            // Check if another brand has the same name
            var exists = await _context.Brands.AnyAsync(b => b.Id != id && b.Name!.ToLower() == dto.Name!.Trim().ToLower());

            if (exists)
            {
                throw new Exception("A brand with this name already exists.");
            }

            brand.Name = dto.Name!.Trim();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBrandAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
        }
    }
}
