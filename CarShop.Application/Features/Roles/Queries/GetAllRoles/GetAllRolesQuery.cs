using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<Result<List<string>>>
    {
    }
}
