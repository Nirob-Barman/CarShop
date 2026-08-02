using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Commands.DeleteRole
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result<bool>>
    {
        private readonly IRoleManager _roleManager;

        public DeleteRoleCommandHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var roleName = request.RoleName;

            if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                roleName.Equals("User", StringComparison.OrdinalIgnoreCase))
                return Result<bool>.Fail($"The '{roleName}' role cannot be deleted.");

            var result = await _roleManager.DeleteRoleAsync(roleName);
            if (!result.Succeeded)
                return Result<bool>.Fail(result.Errors, "Failed to delete role.");

            return Result<bool>.Ok(true, $"Role '{roleName}' deleted successfully.");
        }
    }
}
