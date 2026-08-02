using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Users.Commands.AssignRoleToUser
{
    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, Result<bool>>
    {
        private readonly IUserManager _userManager;

        public AssignRoleToUserCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Result<bool>.Fail("User not found.");

            // Remove existing roles
            var existingRoles = await _userManager.GetRolesAsync(user);
            var removalResult = await _userManager.RemoveFromRoleAsync(user, existingRoles.FirstOrDefault()!);

            if (!removalResult.Succeeded)
            {
                return Result<bool>.Fail(removalResult.Errors, "Failed to remove existing roles.");
            }

            // Add new role
            var addResult = await _userManager.AddToRoleAsync(user, request.RoleName);
            if (!addResult.Succeeded)
            {
                return Result<bool>.Fail(addResult.Errors, "Failed to assign new role.");
            }

            return Result<bool>.Ok(true, $"Role '{request.RoleName}' assigned successfully.");
        }
    }
}
