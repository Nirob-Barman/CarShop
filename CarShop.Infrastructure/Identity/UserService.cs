using CarShop.Application.DTOs.Identity;
using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CarShop.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<string> RegisterAsync(RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Address = model.Address
            };

            var result = await _userManager.CreateAsync(user, model.Password!);
            return result.Succeeded ? user.Id : throw new Exception("Registration failed");
        }

        public async Task<string> LoginAsync(LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email!, model.Password!, false, false);
            return result.Succeeded ? "Success" : throw new Exception("Login failed");
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<EditProfileDto> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return new EditProfileDto { FullName = user!.FullName, Address = user.Address };
        }

        public async Task UpdateProfileAsync(string userId, EditProfileDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            user!.FullName = model.FullName;
            user.Address = model.Address;
            await _userManager.UpdateAsync(user);
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordDto model)
        {
            if (string.IsNullOrWhiteSpace(model.CurrentPassword) ||
                string.IsNullOrWhiteSpace(model.NewPassword))
            {
                throw new Exception("Password fields cannot be empty.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Password change failed: {errorMessage}");
            }

            // Optional: Re-sign in to refresh security stamp/cookies
            await _signInManager.RefreshSignInAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found.");

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found.");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Password reset failed: {errorMessage}");
            }
        }

    }
}
