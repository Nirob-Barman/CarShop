using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Roles.Queries.GetAllRolesNonAdmin
{
    public record GetAllRolesNonAdminQuery
        : IRequest<Result<List<string>>>;
}
