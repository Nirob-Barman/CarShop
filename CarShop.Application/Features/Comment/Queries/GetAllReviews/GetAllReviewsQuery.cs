using CarShop.Application.DTOs.Comment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Queries.GetAllReviews
{
    public class GetAllReviewsQuery : IRequest<Result<IEnumerable<CommentDto>>>
    {
    }
}
