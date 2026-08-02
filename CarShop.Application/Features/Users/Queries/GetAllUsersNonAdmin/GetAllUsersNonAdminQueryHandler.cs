using CarShop.Application.DTOs.Identity;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Users.Queries.GetAllUsersNonAdmin
{
    public class GetAllUsersNonAdminQueryHandler : IRequestHandler<GetAllUsersNonAdminQuery, Result<List<UserWithRoleDto>>>
    {
        private readonly IUserManager _userManager;

        public GetAllUsersNonAdminQueryHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<List<UserWithRoleDto>>> Handle(GetAllUsersNonAdminQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var allUsers = await _userManager.GetAllUsersAsync();
                var nonAdminUsers = new List<UserWithRoleDto>();

                foreach (var user in allUsers)
                {
                    bool isAdmin = await _userManager.IsUserInRoleAsync(user, "Admin");
                    if (isAdmin) continue;

                    var roles = await _userManager.GetRolesAsync(user);

                    nonAdminUsers.Add(new UserWithRoleDto
                    {
                        UserId = user.Id!,
                        Email = user.Email!,
                        FullName = user.FullName!,
                        Address = user.Address,
                        CurrentRole = roles.FirstOrDefault() ?? "None",
                        IsBanned = user.IsBanned,
                    });
                }

                return Result<List<UserWithRoleDto>>.Ok(nonAdminUsers);
            }
            catch (Exception ex)
            {
                return Result<List<UserWithRoleDto>>.Fail("An error occurred while fetching users.", ex.Message);
            }
        }
    }
}
