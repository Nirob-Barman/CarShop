using CarShop.Application.DTOs.Comment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Queries.GetCommentsByCarId
{
    public class GetCommentsByCarIdQuery : IRequest<Result<IEnumerable<CommentDto>>>
    {
        public int CarId { get; set; }

        public GetCommentsByCarIdQuery(int carId)
        {
            CarId = carId;
        }
    }
}
