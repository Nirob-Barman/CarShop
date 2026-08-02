using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Queries.HasUserReviewed
{
    public class HasUserReviewedQuery : IRequest<Result<bool>>
    {
        public int CarId { get; set; }

        public HasUserReviewedQuery(int carId)
        {
            CarId = carId;
        }
    }
}
