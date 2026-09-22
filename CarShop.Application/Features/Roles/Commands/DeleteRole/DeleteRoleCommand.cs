using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Commands.DeleteRole
{
    public record DeleteRoleCommand(string RoleName)
        : IRequest<Result<bool>>;
}
