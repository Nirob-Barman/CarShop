using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleCommand : IRequest<Result<bool>>
    {
        public string RoleName { get; set; }

        public CreateRoleCommand(string roleName)
        {
            RoleName = roleName;
        }
    }
}
