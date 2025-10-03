
using CarShop.Application.DTOs.Comment;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface ICommentService
    {
        Task<Result<IEnumerable<CommentDto>>> GetCommentsByCarIdAsync(int carId);
        Task<Result<string>> AddCommentAsync(int carId, string userName, string content);

        //Task<IEnumerable<CommentDto>> GetCommentsByCarIdAsync(int carId);
        //Task AddCommentAsync(int carId, string userName, string content);
    }
}
