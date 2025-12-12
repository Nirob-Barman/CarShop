using CarShop.Application.DTOs.Identity;
using CarShop.Web.ViewModels.Account;

namespace CarShop.Web.ViewModels.Mappers
{
    public static class AccountMapper
    {
        public static RegisterDto ToDto(RegisterViewModel vm)
        => new RegisterDto
        {
            FullName = vm.FullName,
            Email = vm.Email,
            Password = vm.Password,
        };

        public static LoginDto ToDto(LoginViewModel vm)
            => new LoginDto
            {
                Email = vm.Email,
                Password = vm.Password,
            };

        public static EditProfileDto ToDto(ProfileViewModel vm)
            => new EditProfileDto
            {
                FullName = vm?.FullName,
                Address = vm?.Address,
            };

        public static ChangePasswordDto ToDto(ChangePasswordViewModel vm)
            => new ChangePasswordDto
            {
                CurrentPassword = vm.CurrentPassword,
                NewPassword = vm.NewPassword,
                ConfirmPassword = vm.ConfirmPassword
            };

        public static ForgotPasswordDto ToDto(ForgotPasswordViewModel vm)
            => new ForgotPasswordDto { Email = vm.Email };

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
            };
    }
}
