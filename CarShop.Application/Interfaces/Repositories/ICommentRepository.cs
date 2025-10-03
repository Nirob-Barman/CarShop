

using CarShop.Domain.Entities;

namespace CarShop.Application.Interfaces.Repositories
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetByCarIdAsync(int carId);
        Task AddAsync(Comment comment);
        Task SaveChangesAsync();
    }
}
