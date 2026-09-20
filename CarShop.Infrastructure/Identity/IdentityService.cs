using CarShop.Application.Interfaces.Identity;
using CarShop.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool Succeeded, string? UserId, List<string> Errors)> CreateAsync(AppUser user, string password)
        {
            var identityUser = CreateIdentityUser(user);
            var result = await _userManager.CreateAsync(identityUser, password);
            return result.Succeeded
                ? (true, identityUser.Id, [])
                : (false, null, result.Errors.Select(error => error.Description).ToList());
        }

        public async Task<(bool Succeeded, List<string> Errors)> UpdateAsync(AppUser user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            if (identityUser == null) return (false, ["User not found."]);

            identityUser.FullName = user.FullName;
            identityUser.Address = user.Address;
            return ToOperationResult(await _userManager.UpdateAsync(identityUser));
        }

        public async Task<(bool Succeeded, List<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user == null
                ? (false, ["User not found."])
                : ToOperationResult(await _userManager.ChangePasswordAsync(user, currentPassword, newPassword));
        }

        public async Task<string> GeneratePasswordResetTokenAsync(AppUser user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            return identityUser == null ? string.Empty : await _userManager.GeneratePasswordResetTokenAsync(identityUser);
        }

        public async Task<(bool Succeeded, List<string> Errors)> ResetPasswordAsync(AppUser user, string token, string newPassword)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            return identityUser == null
                ? (false, ["User not found."])
                : ToOperationResult(await _userManager.ResetPasswordAsync(identityUser, token, newPassword));
        }

        public async Task<AppUser?> FindByEmailAsync(string email)
            => ToAppUser(await _userManager.FindByEmailAsync(email));

        public async Task<AppUser?> FindByIdAsync(string id)
            => ToAppUser(await _userManager.FindByIdAsync(id));

        public async Task<string[]> GetRolesAsync(AppUser user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            return identityUser == null ? [] : (await _userManager.GetRolesAsync(identityUser)).ToArray();
        }

        public async Task<bool> CheckPasswordSignInAsync(AppUser user, string password)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            return identityUser != null && (await _signInManager.CheckPasswordSignInAsync(identityUser, password, false)).Succeeded;
        }

        public async Task SignInAsync(AppUser user, bool isPersistent)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            if (identityUser != null) await _signInManager.SignInAsync(identityUser, isPersistent);
        }

        public Task SignOutAsync() => _signInManager.SignOutAsync();

        public async Task RefreshSignInAsync(AppUser user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            if (identityUser != null) await _signInManager.RefreshSignInAsync(identityUser);
        }

        public async Task<(bool Succeeded, List<string> Errors)> AddToRoleAsync(AppUser user, string roleName)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            return identityUser == null ? (false, ["User not found."]) : ToOperationResult(await _userManager.AddToRoleAsync(identityUser, roleName));
        }

        public async Task<(bool Succeeded, List<string> Errors)> RemoveFromRoleAsync(AppUser user, string roleName)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            return identityUser == null ? (false, ["User not found."]) : ToOperationResult(await _userManager.RemoveFromRoleAsync(identityUser, roleName));
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
            => (await _userManager.Users.ToListAsync()).Select(ToAppUser).Where(user => user != null).Cast<AppUser>().ToList();

        public async Task<bool> IsUserInRoleAsync(AppUser user, string role)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id!);
            return identityUser != null && await _userManager.IsInRoleAsync(identityUser, role);
        }

        public async Task<(bool Succeeded, List<string> Errors)> SetLockoutAsync(string userId, bool ban)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return (false, ["User not found."]);

            user.LockoutEnabled = ban;
            user.LockoutEnd = ban ? DateTimeOffset.MaxValue : null;
            return ToOperationResult(await _userManager.UpdateAsync(user));
        }

        public async Task<(bool Succeeded, bool IsNewUser, string? UserId, List<string> Errors)> ExternalLoginSignInAsync(
            string email, string fullName, string provider, string providerKey, string? providerDisplayName)
        {
            var externalResult = await _signInManager.ExternalLoginSignInAsync(provider, providerKey, false, true);
            if (externalResult.Succeeded) return (true, false, null, []);

            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser != null)
            {
                var linkResult = await _userManager.AddLoginAsync(identityUser, new UserLoginInfo(provider, providerKey, providerDisplayName ?? provider));
                if (!linkResult.Succeeded) return (false, false, null, linkResult.Errors.Select(error => error.Description).ToList());

                await _signInManager.SignInAsync(identityUser, false);
                return (true, false, identityUser.Id, []);
            }

            identityUser = CreateIdentityUser(new AppUser { Email = email, FullName = fullName });
            identityUser.EmailConfirmed = true;
            var createResult = await _userManager.CreateAsync(identityUser);
            if (!createResult.Succeeded) return (false, false, null, createResult.Errors.Select(error => error.Description).ToList());

            var roleResult = await _userManager.AddToRoleAsync(identityUser, "User");
            if (!roleResult.Succeeded) return (false, false, identityUser.Id, roleResult.Errors.Select(error => error.Description).ToList());

            var addLoginResult = await _userManager.AddLoginAsync(identityUser, new UserLoginInfo(provider, providerKey, providerDisplayName ?? provider));
            if (!addLoginResult.Succeeded) return (false, false, identityUser.Id, addLoginResult.Errors.Select(error => error.Description).ToList());

            await _signInManager.SignInAsync(identityUser, false);
            return (true, true, identityUser.Id, []);
        }

        private static ApplicationUser CreateIdentityUser(AppUser user) => new()
        {
            Email = user.Email, UserName = user.Email, FullName = user.FullName, Address = user.Address
        };

        private static AppUser? ToAppUser(ApplicationUser? user) => user == null ? null : new AppUser
        {
            Id = user.Id, Email = user.Email!, FullName = user.FullName, Address = user.Address,
            IsBanned = user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow
        };

        private static (bool Succeeded, List<string> Errors) ToOperationResult(IdentityResult result)
            => (result.Succeeded, result.Errors.Select(error => error.Description).ToList());
    }
}
