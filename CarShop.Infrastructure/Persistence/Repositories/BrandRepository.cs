using CarShop.Application.Interfaces.Repositories;
using CarShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Persistence.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;

        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task<Brand?> GetByNameAsync(string name)
        {
            return await _context.Brands.FirstOrDefaultAsync(b => b.Name == name);
        }

        public async Task<int?> GetIdByNameAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();
            var brand = await _context.Brands
                .Where(b => b.Name!.ToLower() == normalizedName)
                .Select(b => new { b.Id })
                .FirstOrDefaultAsync();

            return brand?.Id;
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var normalized = name.Trim().ToLower();
            return await _context.Brands.AnyAsync(b =>
                b.Name!.ToLower() == normalized &&
                (!excludeId.HasValue || b.Id != excludeId.Value));
        }

        public void AddAsync(Brand brand)
        {
            _context.Brands.Add(brand);
        }

        public void UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
        }

        public void DeleteAsync(Brand brand)
        {
            _context.Brands.Remove(brand);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
