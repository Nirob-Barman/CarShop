using CarShop.Application.DTOs.Comment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Queries.GetRecentTestimonials
{
    public class GetRecentTestimonialsQuery : IRequest<Result<IEnumerable<CommentDto>>>
    {
        public int Count { get; set; }

        public GetRecentTestimonialsQuery(int count = 6)
        {
            Count = count;
        }
    }
}
