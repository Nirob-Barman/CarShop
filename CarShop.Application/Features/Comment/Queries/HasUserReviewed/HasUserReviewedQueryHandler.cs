using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Comment.Queries.HasUserReviewed
{
    public class HasUserReviewedQueryHandler : IRequestHandler<HasUserReviewedQuery, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public HasUserReviewedQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<bool>> Handle(HasUserReviewedQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var hasReviewed = await _context.Comments.AnyAsync(
                c => c.CarId == request.CarId && c.UserId == userId && c.Rating.HasValue);

            return Result<bool>.Ok(hasReviewed);
        }
    }
}
