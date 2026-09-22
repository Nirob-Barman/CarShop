using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Commands.CreateRole
{
    public record CreateRoleCommand(string RoleName)
        : IRequest<Result<bool>>;
}
