using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Users.Commands.AssignRoleToUser
{
    public class AssignRoleToUserCommand : IRequest<Result<bool>>
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }

        public AssignRoleToUserCommand(string userId, string roleName)
        {
            UserId = userId;
            RoleName = roleName;
        }
    }
}
