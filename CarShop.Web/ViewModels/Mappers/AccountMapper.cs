using CarShop.Application.DTOs.Identity;
using CarShop.Web.ViewModels.Account;

namespace CarShop.Web.ViewModels.Mappers
{
    public static class AccountMapper
    {
        public static ProfileViewModel ToViewModel(EditProfileDto dto)
            => new ProfileViewModel
            {
                FullName = dto.FullName,
                Address = dto.Address,
                Email = dto.Email,
            };
    }
}
