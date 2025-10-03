using CarShop.Domain.Entities;

namespace CarShop.Application.Interfaces.Repositories
{
    public interface IBrandRepository
    {
        Task<List<Brand>> GetAllAsync();
        Task<Brand?> GetByIdAsync(int id);
        Task<Brand?> GetByNameAsync(string name);
        Task<int?> GetIdByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        void AddAsync(Brand brand);
        void UpdateAsync(Brand brand);
        void DeleteAsync(Brand brand);
        Task SaveChangesAsync();
    }
}
