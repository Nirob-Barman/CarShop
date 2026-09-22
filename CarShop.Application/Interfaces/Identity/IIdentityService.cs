using CarShop.Domain.Entities;

namespace CarShop.Application.Interfaces.Identity
{    
    public interface IIdentityService
    {
        Task<(bool Succeeded, string? UserId, List<string> Errors)> CreateAsync(AppUser user, string password);
        Task<(bool Succeeded, List<string> Errors)> UpdateAsync(AppUser user);
        Task<(bool Succeeded, List<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(AppUser user);
        Task<(bool Succeeded, List<string> Errors)> ResetPasswordAsync(AppUser user, string token, string newPassword);
        Task<AppUser?> FindByEmailAsync(string email);
        Task<AppUser?> FindByIdAsync(string id);
        Task<string[]> GetRolesAsync(AppUser user);
        Task<bool> CheckPasswordSignInAsync(AppUser user, string password);
        Task SignInAsync(AppUser user, bool isPersistent);
        Task SignOutAsync();
        Task RefreshSignInAsync(AppUser user);
        Task<(bool Succeeded, List<string> Errors)> AddToRoleAsync(AppUser user, string roleName);
        Task<(bool Succeeded, List<string> Errors)> RemoveFromRoleAsync(AppUser user, string roleName);
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<bool> IsUserInRoleAsync(AppUser user, string role);
        Task<(bool Succeeded, List<string> Errors)> SetLockoutAsync(string userId, bool ban);
        Task<(bool Succeeded, bool IsNewUser, string? UserId, List<string> Errors)> ExternalLoginSignInAsync(
            string email, string fullName, string provider, string providerKey, string? providerDisplayName);
    }
}
