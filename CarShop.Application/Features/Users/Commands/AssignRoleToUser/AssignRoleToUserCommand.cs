using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Users.Commands.AssignRoleToUser
{
    public record AssignRoleToUserCommand(string UserId, string RoleName)
        : IRequest<Result<bool>>;
}
