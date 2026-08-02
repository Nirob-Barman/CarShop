using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<List<string>>>
    {
        private readonly IRoleManager _roleManager;

        public GetAllRolesQueryHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result<List<string>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleManager.GetAllRolesAsync();
            return Result<List<string>>.Ok(roles);
        }
    }
}
