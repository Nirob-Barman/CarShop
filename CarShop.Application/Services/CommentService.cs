using CarShop.Application.DTOs.Comment;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<CommentDto>>> GetCommentsByCarIdAsync(int carId)
        {
            var comments = await _unitOfWork.Repository<Comment>().GetAllAsync(c => c.CarId == carId, c => new
            {
                c.Id,
                c.UserName,
                c.Content,
                c.CreatedAt,
                c.CarId,
            });

            var dtos = comments.Select(c => new CommentDto
            {
                Id = c.Id,
                UserName = c.UserName,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                CarId = c.CarId
            });

            return Result<IEnumerable<CommentDto>>.Ok(dtos);
        }

        public async Task<Result<string>> AddCommentAsync(int carId, string userName, string content)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(content))
            {
                return Result<string>.Fail("Username and content cannot be empty.");
            }

            var comment = new Comment
            {
                CarId = carId,
                UserName = userName.Trim(),
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Comment>().AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "Comment added successfully.");
        }
    }
}
