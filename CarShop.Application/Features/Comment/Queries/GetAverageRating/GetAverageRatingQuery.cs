using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Queries.GetAverageRating
{
    public class GetAverageRatingQuery : IRequest<Result<double>>
    {
        public int CarId { get; set; }

        public GetAverageRatingQuery(int carId)
        {
            CarId = carId;
        }
    }
}
