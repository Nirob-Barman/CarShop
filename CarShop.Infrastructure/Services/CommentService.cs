using CarShop.Application.DTOs.Comment;
using CarShop.Application.Interfaces;
using CarShop.Domain.Entities;
using CarShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByCarIdAsync(int carId)
        {
            return await _context.Comments
                .Where(c => c.CarId == carId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    UserName = c.UserName ?? "Anonymous",
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    CarId = c.CarId
                })
                .ToListAsync();
        }

        public async Task AddCommentAsync(int carId, string userName, string content)
        {
            var comment = new Comment
            {
                CarId = carId,
                UserName = userName,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }
    }
}