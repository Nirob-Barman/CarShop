using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Queries.EmailExists
{
    public record EmailExistsQuery(string Email) : IRequest<Result<bool>>;
}
