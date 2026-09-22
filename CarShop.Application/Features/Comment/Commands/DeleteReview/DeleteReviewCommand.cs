using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Commands.DeleteReview
{
    public record DeleteReviewCommand(int CommentId) : IRequest<Result<string>>;
}
