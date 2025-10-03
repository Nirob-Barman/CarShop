using CarShop.Application.Interfaces.Repositories;
using CarShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Persistence.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly AppDbContext _context;

        public CarRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Car>> GetAllAsync()
        {
            return await _context.Cars.Include(c => c.Brand).ToListAsync();
        }

        public async Task<List<Car>> GetByBrandIdAsync(int brandId)
        {
            return await _context.Cars
                .Include(c => c.Brand)
                .Where(c => c.BrandId == brandId)
                .ToListAsync();
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            return await _context.Cars.Include(c => c.Brand).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
        }

        public void DeleteAsync(Car car)
        {
            _context.Cars.Remove(car);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
