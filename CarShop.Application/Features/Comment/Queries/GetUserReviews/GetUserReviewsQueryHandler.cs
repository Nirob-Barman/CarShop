using CarShop.Application.DTOs.Comment;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Comment.Queries.GetUserReviews
{
    public class GetUserReviewsQueryHandler : IRequestHandler<GetUserReviewsQuery, Result<IEnumerable<CommentDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetUserReviewsQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<CommentDto>>> Handle(GetUserReviewsQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var reviews = await _context.Comments.Include(c => c.Car).Where(c => c.UserId == userId && c.Rating.HasValue).Select(c => new CommentDto
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
                }).ToListAsync(cancellationToken);

            return Result<IEnumerable<CommentDto>>.Ok(
                reviews.OrderByDescending(c => c.CreatedAt));
        }
    }
}
