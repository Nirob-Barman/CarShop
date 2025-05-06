
using CarShop.Application.DTOs.Identity;

namespace CarShop.Infrastructure.Identity.Mappers
{
    public static class UserMapper
    {
        public static ApplicationUserDto ToDto(ApplicationUser user)
        {
            return new ApplicationUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address
            };
        }
    }
}
