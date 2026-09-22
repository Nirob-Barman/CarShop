using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Commands.RenameRole
{
    public record RenameRoleCommand(string CurrentName, string NewName)
        : IRequest<Result<bool>>;
}
