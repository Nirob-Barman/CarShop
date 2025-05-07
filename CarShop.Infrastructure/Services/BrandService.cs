
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

        public async Task<int> CreateBrandAsync(BrandDto dto)
        {
            var brand = new Brand { Name = dto.Name };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return brand.Id;
        }

        public async Task UpdateBrandAsync(int id, BrandDto dto)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                brand.Name = dto.Name;
                await _context.SaveChangesAsync();
            }
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
