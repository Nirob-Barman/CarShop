using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Users.Queries.GetAllUsersNonAdmin
{
    public class GetAllUsersNonAdminQuery : IRequest<Result<List<UserWithRoleDto>>>
    {
    }
}
