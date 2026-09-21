using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Comment;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Comment.Queries.GetAllReviews
{
    public class GetAllReviewsQueryHandler : IRequestHandler<GetAllReviewsQuery, Result<IEnumerable<CommentDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllReviewsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<CommentDto>>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _context.Comments.AsNoTracking().Include(c => c.Car).Where(c => c.Rating.HasValue).Select(c => new CommentDto
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
                }).ToListAsync(cancellationToken);

            return Result<IEnumerable<CommentDto>>.Ok(
                reviews.OrderByDescending(c => c.CreatedAt));
        }
    }
}
