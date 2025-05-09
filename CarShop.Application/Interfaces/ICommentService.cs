
using CarShop.Application.DTOs.Comment;

namespace CarShop.Application.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetCommentsByCarIdAsync(int carId);
        Task AddCommentAsync(int carId, string userName, string content);
    }
}
