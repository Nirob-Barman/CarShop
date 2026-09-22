using CarShop.Application.DTOs.Identity;
using CarShop.Web.ViewModels.Account;

namespace CarShop.Web.ViewModels.Mappers
{
    public static class AccountMapper
    {
        public static ChangePasswordDto ToDto(ChangePasswordViewModel vm)
            => new ChangePasswordDto
            {
                CurrentPassword = vm.CurrentPassword,
                NewPassword = vm.NewPassword,
                ConfirmPassword = vm.ConfirmPassword
            };

        public static ResetPasswordDto ToDto(ResetPasswordViewModel vm)
            => new ResetPasswordDto
            {
                Email = vm.Email,
                Token = vm.Token,
                NewPassword = vm.NewPassword,
            };

        public static ProfileViewModel ToViewModel(EditProfileDto dto)
            => new ProfileViewModel
            {
                FullName = dto.FullName,
                Address = dto.Address,
                Email = dto.Email,
            };
    }
}
