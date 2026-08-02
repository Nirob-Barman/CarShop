using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using CommentEntity = CarShop.Domain.Entities.Comment;

namespace CarShop.Application.Features.Comment.Queries.HasUserReviewed
{
    public class HasUserReviewedQueryHandler : IRequestHandler<HasUserReviewedQuery, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public HasUserReviewedQueryHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<bool>> Handle(HasUserReviewedQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var hasReviewed = await _unitOfWork.Repository<CommentEntity>().AnyAsync(
                c => c.CarId == request.CarId && c.UserId == userId && c.Rating.HasValue);

            return Result<bool>.Ok(hasReviewed);
        }
    }
}
