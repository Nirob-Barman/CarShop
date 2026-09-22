using CarShop.Application.DTOs.Comment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Queries.GetCommentsByCarId
{
    public record GetCommentsByCarIdQuery(int CarId) : IRequest<Result<IEnumerable<CommentDto>>>;
}
