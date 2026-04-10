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
        private readonly IUserContextService _userContextService;

        public CommentService(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<CommentDto>>> GetCommentsByCarIdAsync(int carId)
        {
            var comments = await _unitOfWork.Repository<Comment>().GetAllAsync(c => c.CarId == carId, c => new CommentDto
            {
                Id = c.Id,
                UserName = c.UserName,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                CarId = c.CarId,
                Rating = c.Rating,
                UserId = c.UserId
            });

            return Result<IEnumerable<CommentDto>>.Ok(comments);
        }

        public async Task<Result<string>> AddCommentAsync(int carId, string userName, string content)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(content))
                return Result<string>.Fail("Username and content cannot be empty.");

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

        public async Task<Result<double>> GetAverageRatingAsync(int carId)
        {
            var comments = await _unitOfWork.Repository<Comment>().GetAllAsync(
                c => c.CarId == carId && c.Rating.HasValue,
                c => c.Rating!.Value);

            var ratingList = comments.ToList();
            if (!ratingList.Any())
                return Result<double>.Ok(0);

            return Result<double>.Ok(ratingList.Average());
        }

        public async Task<Result<string>> AddReviewAsync(int carId, string content, int rating)
        {
            var userId   = _userContextService.UserId!;
            var userName = _userContextService.Email ?? "User";

            if (string.IsNullOrWhiteSpace(content))
                return Result<string>.Fail("Review content cannot be empty.");

            if (rating < 1 || rating > 5)
                return Result<string>.Fail("Rating must be between 1 and 5.");

            var alreadyReviewed = await _unitOfWork.Repository<Comment>().AnyAsync(
                c => c.CarId == carId && c.UserId == userId && c.Rating.HasValue);

            if (alreadyReviewed)
                return Result<string>.Fail("You have already reviewed this car.");

            var comment = new Comment
            {
                CarId = carId,
                UserId = userId,
                UserName = userName.Trim(),
                Content = content.Trim(),
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Comment>().AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "Review added successfully.");
        }

        public async Task<Result<bool>> HasUserReviewedAsync(int carId)
        {
            var userId = _userContextService.UserId!;
            var hasReviewed = await _unitOfWork.Repository<Comment>().AnyAsync(
                c => c.CarId == carId && c.UserId == userId && c.Rating.HasValue);

            return Result<bool>.Ok(hasReviewed);
        }

        public async Task<Result<string>> EditReviewAsync(int commentId, string content, int rating)
        {
            var userId  = _userContextService.UserId!;
            var isAdmin = _userContextService.IsInRole("Admin");

            if (rating < 1 || rating > 5)
                return Result<string>.Fail("Rating must be between 1 and 5.");

            var comment = await _unitOfWork.Repository<Comment>().FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null)
                return Result<string>.Fail("Review not found.");

            if (!isAdmin && comment.UserId != userId)
                return Result<string>.Fail("You are not allowed to edit this review.");

            comment.Content = content.Trim();
            comment.Rating = rating;
            _unitOfWork.Repository<Comment>().Update(comment);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "Review updated successfully.");
        }

        public async Task<Result<string>> DeleteReviewAsync(int commentId)
        {
            var userId  = _userContextService.UserId!;
            var isAdmin = _userContextService.IsInRole("Admin");

            var comment = await _unitOfWork.Repository<Comment>().FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null)
                return Result<string>.Fail("Review not found.");

            if (!isAdmin && comment.UserId != userId)
                return Result<string>.Fail("You are not allowed to delete this review.");

            _unitOfWork.Repository<Comment>().Remove(comment);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "Review deleted.");
        }

        public async Task<Result<IEnumerable<CommentDto>>> GetUserReviewsAsync()
        {
            var userId = _userContextService.UserId!;
            var reviews = await _unitOfWork.Repository<Comment>().GetAllWithIncludesAsync(
                predicate: c => c.UserId == userId && c.Rating.HasValue,
                selector: c => new CommentDto
                {
                    Id           = c.Id,
                    UserName     = c.UserName,
                    Content      = c.Content,
                    CreatedAt    = c.CreatedAt,
                    CarId        = c.CarId,
                    Rating       = c.Rating,
                    UserId       = c.UserId,
                    CarTitle     = c.Car != null ? c.Car.Title : null,
                    CarImageUrl  = c.Car != null ? c.Car.ImageUrl : null
                },
                c => c.Car!
            );

            return Result<IEnumerable<CommentDto>>.Ok(
                reviews.OrderByDescending(c => c.CreatedAt));
        }

        public async Task<Result<IEnumerable<CommentDto>>> GetAllReviewsAsync()
        {
            var reviews = await _unitOfWork.Repository<Comment>().GetAllWithIncludesAsync(
                predicate: c => c.Rating.HasValue,
                selector: c => new CommentDto
                {
                    Id          = c.Id,
                    UserName    = c.UserName,
                    Content     = c.Content,
                    CreatedAt   = c.CreatedAt,
                    CarId       = c.CarId,
                    Rating      = c.Rating,
                    UserId      = c.UserId,
                    CarTitle    = c.Car != null ? c.Car.Title : null,
                    CarImageUrl = c.Car != null ? c.Car.ImageUrl : null
                },
                c => c.Car!
            );

            return Result<IEnumerable<CommentDto>>.Ok(
                reviews.OrderByDescending(c => c.CreatedAt));
        }

        public async Task<Result<IEnumerable<CommentDto>>> GetRecentTestimonialsAsync(int count = 6)
        {
            var comments = await _unitOfWork.Repository<Comment>().GetAllWithIncludesAsync(
                predicate: c => c.Rating.HasValue && c.Rating >= 4 && !string.IsNullOrEmpty(c.Content),
                selector: c => new CommentDto
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    CarId = c.CarId,
                    Rating = c.Rating,
                    UserId = c.UserId,
                    CarTitle = c.Car != null ? c.Car.Title : null
                },
                c => c.Car!
            );

            var result = comments.OrderByDescending(c => c.CreatedAt).Take(count);
            return Result<IEnumerable<CommentDto>>.Ok(result);
        }
    }
}
