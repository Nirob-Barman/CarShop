using CarShop.Application.DTOs.Comment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Queries.GetUserReviews
{
    public record GetUserReviewsQuery : IRequest<Result<IEnumerable<CommentDto>>>;
}
