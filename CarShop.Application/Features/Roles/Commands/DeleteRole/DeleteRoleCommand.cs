using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Commands.DeleteRole
{
    public class DeleteRoleCommand : IRequest<Result<bool>>
    {
        public string RoleName { get; set; }

        public DeleteRoleCommand(string roleName)
        {
            RoleName = roleName;
        }
    }
}
