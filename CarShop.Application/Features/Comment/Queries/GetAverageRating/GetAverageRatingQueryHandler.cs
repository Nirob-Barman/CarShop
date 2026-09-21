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
            var comments = await _context.Comments.AsNoTracking()
                .Where(c => c.CarId == request.CarId && c.Rating.HasValue)
                .Select(c => c.Rating!.Value)
                .ToListAsync(cancellationToken);

            var ratingList = comments.ToList();
            if (!ratingList.Any())
                return Result<double>.Ok(0);

            return Result<double>.Ok(ratingList.Average());
        }
    }
}
