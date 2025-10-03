
using CarShop.Domain.Entities;

namespace CarShop.Application.Interfaces.Repositories
{
    public interface ICarRepository
    {
        Task<List<Car>> GetAllAsync();
        Task<List<Car>> GetByBrandIdAsync(int brandId);
        Task<Car?> GetByIdAsync(int id);
        Task AddAsync(Car car);
        void DeleteAsync(Car car);
        Task SaveChangesAsync();
    }
}
