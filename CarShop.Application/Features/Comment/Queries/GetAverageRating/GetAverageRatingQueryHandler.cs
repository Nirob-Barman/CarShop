using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Comment.Queries.GetAverageRating
{
    public class GetAverageRatingQueryHandler : IRequestHandler<GetAverageRatingQuery, Result<double>>
    {
        private readonly IApplicationDbContext _context;

        public GetAverageRatingQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<double>> Handle(GetAverageRatingQuery request, CancellationToken cancellationToken)
        {
            var averageRating = await _context.Comments.AsNoTracking()
                .Where(c => c.CarId == request.CarId && c.Rating.HasValue)
                .AverageAsync(c => (double?)c.Rating, cancellationToken);

            return Result<double>.Ok(averageRating ?? 0);
        }
    }
}
