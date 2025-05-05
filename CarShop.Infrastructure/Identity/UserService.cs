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

    }
}
