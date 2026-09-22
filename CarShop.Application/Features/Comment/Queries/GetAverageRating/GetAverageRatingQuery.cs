using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Queries.GetAverageRating
{
    public record GetAverageRatingQuery(int CarId) : IRequest<Result<double>>;
}
