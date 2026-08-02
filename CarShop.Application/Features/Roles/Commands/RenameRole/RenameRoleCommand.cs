using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Commands.RenameRole
{
    public class RenameRoleCommand : IRequest<Result<bool>>
    {
        public string CurrentName { get; set; }
        public string NewName { get; set; }

        public RenameRoleCommand(string currentName, string newName)
        {
            CurrentName = currentName;
            NewName = newName;
        }
    }
}
