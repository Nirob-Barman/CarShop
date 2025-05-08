using CarShop.Application.DTOs.Identity;
using CarShop.Application.Interfaces;
using CarShop.Infrastructure.Identity.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
            //return result.Succeeded ? user.Id : throw new Exception("Registration failed");

            if (!result.Succeeded)
                throw new Exception("Registration failed");

            // Assign default role ("User")
            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                // delete the user if role assignment fails
                await _userManager.DeleteAsync(user);
                throw new Exception("Failed to assign default role to user.");
            }

            return user.Id;
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
        public async Task<bool> CheckPasswordAsync(ApplicationUserDto userDto, string password)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id!);
            return await _userManager.CheckPasswordAsync(user!, password);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
        public async Task<ApplicationUserDto?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user == null ? null : UserMapper.ToDto(user);
        }

        public async Task<ApplicationUserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null ? null : UserMapper.ToDto(user);
        }

        public async Task<EditProfileDto> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return new EditProfileDto { FullName = user!.FullName, Address = user.Address };
        }

        public async Task UpdateProfileAsync(string userId, EditProfileDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");
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
                //var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                //throw new Exception($"Password change failed: {errorMessage}");
                throw new Exception("Password change failed");
            }

            // Re-sign in to refresh security stamp/cookies
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

        public async Task<List<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
        }

        public async Task<List<UserWithRoleDto>> GetAllUsersWithRolesAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserWithRoleDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserWithRoleDto
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName!,
                    Address = user.Address,
                    CurrentRole = roles.FirstOrDefault() ?? "None"
                });
            }

            return result;
        }

        public async Task AssignRoleToUserAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            // Remove existing roles
            var existingRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, existingRoles);

            // Add new role
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                //var error = string.Join(", ", result.Errors.Select(e => e.Description));
                //throw new Exception(error);
                throw new Exception("Role Assign Failed");
            }
        }
        public async Task<List<string>> GetAllRolesNonAdminAsync()
        {
            var allRoles = await _roleManager.Roles
                .Where(r => r.Name != "Admin")
                .Select(r => r.Name!)
                .ToListAsync();

            return allRoles;
        }
        public async Task<List<UserWithRoleDto>> GetAllUsersNonAdminAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserWithRoleDto>();

            foreach (var user in users)
            {
                // Skip if user is Admin
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    continue;

                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserWithRoleDto
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName!,
                    Address = user.Address,
                    CurrentRole = roles.FirstOrDefault() ?? "None"
                });
            }

            return result;
        }
        public async Task CreateRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name is required.");

            roleName = roleName.Trim();

            if (await _roleManager.RoleExistsAsync(roleName))
                throw new Exception($"Role '{roleName}' already exists.");

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(error);
            }
        }
    }
}
