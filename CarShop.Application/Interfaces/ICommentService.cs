
using CarShop.Application.DTOs.Comment;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface ICommentService
    {
        Task<Result<IEnumerable<CommentDto>>> GetCommentsByCarIdAsync(int carId);
        Task<Result<string>> AddCommentAsync(int carId, string userName, string content);
        Task<Result<double>> GetAverageRatingAsync(int carId);
        Task<Result<string>> AddReviewAsync(int carId, string content, int rating);
        Task<Result<bool>> HasUserReviewedAsync(int carId);
        Task<Result<IEnumerable<CommentDto>>> GetRecentTestimonialsAsync(int count = 6);
        Task<Result<string>> EditReviewAsync(int commentId, string content, int rating);
        Task<Result<string>> DeleteReviewAsync(int commentId);
        Task<Result<IEnumerable<CommentDto>>> GetUserReviewsAsync();
        Task<Result<IEnumerable<CommentDto>>> GetAllReviewsAsync();
    }
}
