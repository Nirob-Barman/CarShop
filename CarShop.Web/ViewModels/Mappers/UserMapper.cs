using CarShop.Application.DTOs.Identity;
using CarShop.Web.ViewModels.Admin;

namespace CarShop.Web.ViewModels.Mappers
{
    public static class UserMapper
    {
        public static UserWithRoleViewModel ToViewModel(UserWithRoleDto dto)
        {
            return new UserWithRoleViewModel
            {
                UserId = dto.UserId,
                Email = dto.Email,
                FullName = dto.FullName,
                Address = dto.Address,
                CurrentRole = dto.CurrentRole,
                AllRoles = dto.AllRoles
            };
        }

        public static List<UserWithRoleViewModel> ToViewModels(IEnumerable<UserWithRoleDto> dtos)
        {
            return dtos.Select(ToViewModel).ToList();
        }
    }
}
