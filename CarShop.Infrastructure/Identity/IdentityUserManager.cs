using CarShop.Application.DTOs.Identity;
using CarShop.Application.Interfaces.Identity;
using CarShop.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Identity
{
    public class IdentityUserManager : IUserManager
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityUserManager(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(bool Succeeded, string? UserId, List<string> Errors)> CreateAsync(AppUser user, string password)
        {
            var identityUser = new ApplicationUser
            {
                Email = user.Email,
                UserName = user.Email,
                Address = user.Address,
            };

            var result = await _userManager.CreateAsync(identityUser, password);
            if (result.Succeeded)
            {
                return (true, identityUser.Id, new List<string>());
            }
            else
            {
                return (false, null, result.Errors.Select(e => e.Description).ToList());
            }
        }


        public async Task<(bool Succeeded, List<string> Errors)> UpdateAsync(AppUser user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            if (identityUser == null)
                return (false, new List<string> { "User not found." });

            // Update the properties you want to allow changes for
            identityUser.FullName = user.FullName;
            identityUser.Address = user.Address;

            var result = await _userManager.UpdateAsync(identityUser);

            return (result.Succeeded, result.Errors.Select(e => e.Description).ToList());
        }


        public async Task<(bool Succeeded, List<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, new List<string> { "User not found." });
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                return (false, result.Errors.Select(e => e.Description).ToList());
            }

            return (true, new List<string>());
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("User not found.");

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }


        public async Task<AppUser?> FindByEmailAsync(string email)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser == null) return null;

            return new AppUser
            {
                Id = identityUser.Id,
                Email = identityUser.Email!,
            };
        }

        public async Task<AppUser?> FindByIdAsync(string id)
        {
            var identityUser = await _userManager.FindByIdAsync(id.ToString());
            if (identityUser == null) return null;

            return new AppUser
            {
                Id = identityUser.Id,
                Email = identityUser.Email!,
            };
        }

        public async Task<string[]> GetRolesAsync(AppUser user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!.ToString());
            if (identityUser == null) return Array.Empty<string>();

            var roles = await _userManager.GetRolesAsync(identityUser);
            return roles.ToArray();
        }

        public async Task<bool> CheckPasswordAsync(AppUser user, string password)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!.ToString());
            if (identityUser == null) return false;

            return await _userManager.CheckPasswordAsync(identityUser, password);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(AppUser user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            if (identityUser == null)
                return string.Empty;

            return await _userManager.GeneratePasswordResetTokenAsync(identityUser);
        }

        public async Task<(bool Succeeded, List<string> Errors)> ResetPasswordAsync(AppUser user, string token, string newPassword)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            if (identityUser == null)
                return (false, new List<string> { "User not found." });

            var result = await _userManager.ResetPasswordAsync(identityUser, token, newPassword);

            return (result.Succeeded, result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<(bool Succeeded, List<string> Errors)> AddToRoleAsync(AppUser user, string roleName)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            if (identityUser == null)
                return (false, new List<string> { "User not found." });

            var result = await _userManager.AddToRoleAsync(identityUser, roleName);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<(bool Succeeded, List<string> Errors)> RemoveFromRoleAsync(AppUser user, string roleName)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            if (identityUser == null)
                return (false, new List<string> { "User not found." });

            var result = await _userManager.RemoveFromRoleAsync(identityUser, roleName);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToList());
        }


        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            var identityUsers = await _userManager.Users.ToListAsync();

            return identityUsers.Select(u => new AppUser
            {
                Id = u.Id,
                Email = u.Email!,
                FullName = u.FullName!,
                Address = u.Address
            }).ToList();
        }

        public async Task<bool> IsUserInRoleAsync(AppUser user, string role)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            if (identityUser == null) return false;

            return await _userManager.IsInRoleAsync(identityUser, role);
        }

        //public async Task<List<UserWithRoleDto>> GetAllUsersNonAdminAsync()
        //{
        //    var users = await _userManager.Users.ToListAsync();

        //    var result = new List<UserWithRoleDto>();

        //    foreach (var u in users)
        //    {
        //        var roles = await _userManager.GetRolesAsync(u);
        //        bool isAdmin = roles.Contains("Admin");

        //        if (isAdmin)
        //            continue;

        //        result.Add(new UserWithRoleDto
        //        {
        //            UserId = u.Id,
        //            Email = u.Email!,
        //            FullName = u.FullName!,
        //            Address = u.Address,
        //            CurrentRole = roles.FirstOrDefault() ?? "None"
        //        });
        //    }

        //    return result;
        //}

    }
}
