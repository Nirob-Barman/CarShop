using CarShop.Application.DTOs.Comment;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using CommentEntity = CarShop.Domain.Entities.Comment;

namespace CarShop.Application.Features.Comment.Queries.GetUserReviews
{
    public class GetUserReviewsQueryHandler : IRequestHandler<GetUserReviewsQuery, Result<IEnumerable<CommentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public GetUserReviewsQueryHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<CommentDto>>> Handle(GetUserReviewsQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var reviews = await _unitOfWork.Repository<CommentEntity>().GetAllWithIncludesAsync(
                predicate: c => c.UserId == userId && c.Rating.HasValue,
                selector: c => new CommentDto
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
                },
                c => c.Car!
            );

            return Result<IEnumerable<CommentDto>>.Ok(
                reviews.OrderByDescending(c => c.CreatedAt));
        }
    }
}
