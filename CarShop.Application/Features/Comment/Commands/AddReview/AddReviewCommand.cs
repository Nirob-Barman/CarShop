using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Commands.AddReview
{
    public record AddReviewCommand(
        int CarId,
        string? Content,
        int Rating
    ) : IRequest<Result<string>>;
}
