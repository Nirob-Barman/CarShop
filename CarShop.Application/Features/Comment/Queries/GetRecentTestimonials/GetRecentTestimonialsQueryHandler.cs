using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Comment;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Comment.Queries.GetRecentTestimonials
{
    public class GetRecentTestimonialsQueryHandler : IRequestHandler<GetRecentTestimonialsQuery, Result<IEnumerable<CommentDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetRecentTestimonialsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<CommentDto>>> Handle(GetRecentTestimonialsQuery request, CancellationToken cancellationToken)
        {
            var testimonials = await _context.Comments.AsNoTracking().Where(c => c.Rating >= 4 && !string.IsNullOrEmpty(c.Content)).OrderByDescending(c => c.CreatedAt)
                .Take(request.Count).Select(c => new CommentDto
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    CarId = c.CarId,
                    Rating = c.Rating,
                    UserId = c.UserId,
                    CarTitle = c.Car != null ? c.Car.Title : null
                }).ToListAsync(cancellationToken);

            return Result<IEnumerable<CommentDto>>.Ok(testimonials);
        }
    }
}
