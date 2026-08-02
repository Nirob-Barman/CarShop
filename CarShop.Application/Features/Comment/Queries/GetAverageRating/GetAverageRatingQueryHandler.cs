using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using CommentEntity = CarShop.Domain.Entities.Comment;

namespace CarShop.Application.Features.Comment.Queries.GetAverageRating
{
    public class GetAverageRatingQueryHandler : IRequestHandler<GetAverageRatingQuery, Result<double>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAverageRatingQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<double>> Handle(GetAverageRatingQuery request, CancellationToken cancellationToken)
        {
            var comments = await _unitOfWork.Repository<CommentEntity>().GetAllAsync(
                c => c.CarId == request.CarId && c.Rating.HasValue,
                c => c.Rating!.Value);

            var ratingList = comments.ToList();
            if (!ratingList.Any())
                return Result<double>.Ok(0);

            return Result<double>.Ok(ratingList.Average());
        }
    }
}
