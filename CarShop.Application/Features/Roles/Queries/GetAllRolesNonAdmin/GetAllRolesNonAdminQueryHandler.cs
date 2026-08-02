using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Queries.GetAllRolesNonAdmin
{
    public class GetAllRolesNonAdminQueryHandler : IRequestHandler<GetAllRolesNonAdminQuery, Result<List<string>>>
    {
        private readonly IRoleManager _roleManager;

        public GetAllRolesNonAdminQueryHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result<List<string>>> Handle(GetAllRolesNonAdminQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _roleManager.GetAllRolesAsync(excludeAdmin: true);

                return Result<List<string>>.Ok(roles);
            }
            catch (Exception ex)
            {
                return Result<List<string>>.Fail("Failed to retrieve roles.", ex.Message);
            }
        }
    }
}
