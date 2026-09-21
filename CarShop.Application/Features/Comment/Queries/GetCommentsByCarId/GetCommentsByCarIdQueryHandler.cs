using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Comment;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Comment.Queries.GetCommentsByCarId
{
    public class GetCommentsByCarIdQueryHandler : IRequestHandler<GetCommentsByCarIdQuery, Result<IEnumerable<CommentDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetCommentsByCarIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<CommentDto>>> Handle(GetCommentsByCarIdQuery request, CancellationToken cancellationToken)
        {
            var comments = await _context.Comments.Where(c => c.CarId == request.CarId).Select(c => new CommentDto
            {
                Id = c.Id,
                UserName = c.UserName,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                CarId = c.CarId,
                Rating = c.Rating,
                UserId = c.UserId
            }).ToListAsync(cancellationToken);

            return Result<IEnumerable<CommentDto>>.Ok(comments);
        }
    }
}
