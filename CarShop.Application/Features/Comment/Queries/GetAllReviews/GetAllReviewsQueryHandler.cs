using CarShop.Application.DTOs.Comment;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using CommentEntity = CarShop.Domain.Entities.Comment;

namespace CarShop.Application.Features.Comment.Queries.GetAllReviews
{
    public class GetAllReviewsQueryHandler : IRequestHandler<GetAllReviewsQuery, Result<IEnumerable<CommentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllReviewsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<CommentDto>>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _unitOfWork.Repository<CommentEntity>().GetAllWithIncludesAsync(
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
    }
}
