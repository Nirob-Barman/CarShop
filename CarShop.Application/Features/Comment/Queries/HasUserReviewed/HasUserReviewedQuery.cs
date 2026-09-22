using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Queries.HasUserReviewed
{
    public record HasUserReviewedQuery(int CarId) : IRequest<Result<bool>>;
}
