using CarShop.Application.DTOs.Comment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Queries.GetRecentTestimonials
{
    public record GetRecentTestimonialsQuery(int Count = 6) : IRequest<Result<IEnumerable<CommentDto>>>;
}
