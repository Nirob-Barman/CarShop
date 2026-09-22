using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Commands.EditReview
{
    public record EditReviewCommand(
        int CommentId,
        string? Content,
        int Rating
    ) : IRequest<Result<string>>;
}
