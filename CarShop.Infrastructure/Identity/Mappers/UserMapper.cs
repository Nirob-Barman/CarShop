
using CarShop.Application.DTOs.Identity;

namespace CarShop.Infrastructure.Identity.Mappers
{
    public static class UserMapper
    {
        public static ApplicationUser ToEntity(ApplicationUserDto userDto)
        {
            return new ApplicationUser
            {
                Id = userDto.Id!,
                FullName = userDto.FullName,
                Email = userDto.Email,
                Address = userDto.Address
            };
        }
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
