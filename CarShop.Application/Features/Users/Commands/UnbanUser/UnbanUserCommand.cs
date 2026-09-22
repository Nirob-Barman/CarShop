using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Users.Commands.UnbanUser
{
    public record UnbanUserCommand(string UserId)
        : IRequest<Result<bool>>;
}
