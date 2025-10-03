using CarShop.Application.DTOs.Identity;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserManager _userManager;
        private readonly ISignInManager _signInManager;
        private readonly IRoleManager _roleManager;
        private readonly IEmailService _emailService;

        public UserService(IUserManager userManager, ISignInManager signInManager, IRoleManager roleManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }


        public async Task<Result<string>> RegisterAsync(RegisterDto model)
        {
            var user = new AppUser
            {
                Email = model.Email,
                FullName = model.FullName,
                Address = model.Address
            };

            var (succeeded, userId, errors) = await _userManager.CreateAsync(user, model.Password!);

            if (!succeeded)
                return Result<string>.Fail(errors!, "Registration failed");

            var roleResult = await _userManager.AddToRoleAsync(new AppUser { Id = userId }, "User");

            if (!roleResult.Succeeded)
            {
                // Cleanup: delete user if role assignment fails
                await _userManager.RemoveFromRoleAsync(new AppUser { Id = userId }, "User");

                return Result<string>.Fail("Failed to assign default role to user.");
            }

            var welcomeMessage = $"Hello {model.FullName},<br>Welcome to CarShop! Thank you for registering.";
            await _emailService.SendEmailAsync(model.Email!, "Welcome to CarShop", welcomeMessage);

            return Result<string>.Ok(userId, "Registration successful");
        }


        public async Task<Result<string>> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user == null)
                //return Result<string>.Fail("Invalid credentials");
                return Result<string>.FailField(nameof(model.Email), "This email is not registered.");

            var isPasswordValid = await _signInManager.CheckPasswordSignInAsync(user, model.Password!);
            if (!isPasswordValid)
                //return Result<string>.Fail("Invalid credentials");
                return Result<string>.FailField(nameof(model.Password), "Incorrect password.");

            await _signInManager.SignInAsync(user, isPersistent: false);

            return Result<string>.Ok("Success", "Login successful");
        }


        public async Task<Result<string>> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return Result<string>.Ok("Success", "Logout successful");
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

        public async Task<Result<EditProfileDto>> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Result<EditProfileDto>.Fail("User not found.");

            var dto = new EditProfileDto
            {
                FullName = user.FullName,
                Address = user.Address
            };

            return Result<EditProfileDto>.Ok(dto);
        }



        public async Task<Result<bool>> UpdateProfileAsync(string userId, EditProfileDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<bool>.Fail("User not found.");

            user.FullName = model.FullName;
            user.Address = model.Address;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {                
                return Result<bool>.Fail(updateResult.Errors, "Failed to update profile.");
            }

            return Result<bool>.Ok(true, "Profile updated successfully.");
        }


        public async Task<Result<bool>> ChangePasswordAsync(string userId, ChangePasswordDto model)
        {
            if (string.IsNullOrWhiteSpace(model.CurrentPassword))
            {
                return Result<bool>.FailField(nameof(model.CurrentPassword), "Password fields cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return Result<bool>.FailField(nameof(model.NewPassword), "Password fields cannot be empty.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<bool>.Fail("User not found.");

            var result = await _userManager.ChangePasswordAsync(user.Id!, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return Result<bool>.Fail(result.Errors, "Password change failed.");
            }

            // Re-sign in to refresh security stamp/cookies
            await _signInManager.RefreshSignInAsync(user);

            return Result<bool>.Ok(true, "Password changed successfully.");
        }


        public async Task<Result<string>> GeneratePasswordResetTokenAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result<string>.Fail("Email is required.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<string>.Fail("User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Result<string>.Ok(token);
        }


        public async Task<Result<bool>> ResetPasswordAsync(string email, string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
            {
                return Result<bool>.Fail("Email, token, and new password are required.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<bool>.Fail("User not found.");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                return Result<bool>.Fail(result.Errors, "Password reset failed.");
            }

            return Result<bool>.Ok(true, "Password has been reset successfully.");
        }


        public async Task<List<string>> GetAllRolesAsync()
        {
            return await _roleManager.GetAllRolesAsync();
        }


        public async Task<Result<bool>> AssignRoleToUserAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<bool>.Fail("User not found.");

            // Remove existing roles
            var existingRoles = await _userManager.GetRolesAsync(user);
            var removalResult = await _userManager.RemoveFromRoleAsync(user, existingRoles.FirstOrDefault()!);

            if (!removalResult.Succeeded)
            {
                return Result<bool>.Fail(removalResult.Errors, "Failed to remove existing roles.");
            }

            // Add new role
            var addResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!addResult.Succeeded)
            {
                return Result<bool>.Fail(addResult.Errors, "Failed to assign new role.");
            }

            return Result<bool>.Ok(true, $"Role '{roleName}' assigned successfully.");
        }


        public async Task<Result<List<string>>> GetAllRolesNonAdminAsync()
        {
            try
            {
                var roles = await _roleManager.GetAllRolesAsync(excludeAdmin: true);

                return Result<List<string>>.Ok(roles);
            }
            catch (Exception ex)
            {
                return Result<List<string>>.Fail("Failed to retrieve roles.", ex.Message);
            }
        }


        public async Task<Result<List<UserWithRoleDto>>> GetAllUsersNonAdminAsync()
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
                        CurrentRole = roles.FirstOrDefault() ?? "None"
                    });
                }

                return Result<List<UserWithRoleDto>>.Ok(nonAdminUsers);
            }
            catch (Exception ex)
            {
                return Result<List<UserWithRoleDto>>.Fail("An error occurred while fetching users.", ex.Message);
            }
        }


        public async Task<Result<bool>> CreateRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return Result<bool>.Fail("Role name is required.");

            roleName = roleName.Trim();

            if (await _roleManager.RoleExistsAsync(roleName))
                return Result<bool>.Fail($"Role '{roleName}' already exists.");

            var result = await _roleManager.CreateRoleAsync(roleName);

            if (!result.Succeeded)
            {
                return Result<bool>.Fail(result.Errors, "Failed to create role.");
            }

            return Result<bool>.Ok(true, $"Role '{roleName}' created successfully.");
        }
    }
}
