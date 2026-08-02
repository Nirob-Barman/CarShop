using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<bool>>
    {
        private readonly IRoleManager _roleManager;

        public CreateRoleCommandHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result<bool>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var roleName = request.RoleName;

            if (string.IsNullOrWhiteSpace(roleName))
                return Result<bool>.Fail("Role name is required.");

            roleName = roleName.Trim();

            if (await _roleManager.RoleExistsAsync(roleName))
                return Result<bool>.Fail($"Role '{roleName}' already exists.");

            var result = await _roleManager.CreateRoleAsync(roleName);

            if (!result.Succeeded)
            {
                return Result<bool>.Fail(result.Errors, "Failed to create role.");
            }

            return Result<bool>.Ok(true, $"Role '{roleName}' created successfully.");
        }
    }
}
