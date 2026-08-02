using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Queries.GetAllRolesNonAdmin
{
    public class GetAllRolesNonAdminQuery : IRequest<Result<List<string>>>
    {
    }
}
