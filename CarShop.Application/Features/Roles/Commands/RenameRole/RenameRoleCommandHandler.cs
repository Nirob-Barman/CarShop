using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Commands.RenameRole
{
    public class RenameRoleCommandHandler : IRequestHandler<RenameRoleCommand, Result<bool>>
    {
        private readonly IRoleManager _roleManager;

        public RenameRoleCommandHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result<bool>> Handle(RenameRoleCommand request, CancellationToken cancellationToken)
        {
            var newName = request.NewName;

            if (string.IsNullOrWhiteSpace(newName))
                return Result<bool>.Fail("New role name is required.");

            newName = newName.Trim();

            if (request.CurrentName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return Result<bool>.Fail("The Admin role cannot be renamed.");

            var result = await _roleManager.RenameRoleAsync(request.CurrentName, newName);
            if (!result.Succeeded)
                return Result<bool>.Fail(result.Errors, "Failed to rename role.");

            return Result<bool>.Ok(true, $"Role renamed to '{newName}' successfully.");
        }
    }
}
