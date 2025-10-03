

using CarShop.Application.Interfaces.Repositories;
using CarShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Persistence.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetByCarIdAsync(int carId)
        {
            return await _context.Comments
                .Where(c => c.CarId == carId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
