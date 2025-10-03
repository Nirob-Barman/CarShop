using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Interfaces.Identity
{
    public interface IUserManager
    {
        Task<(bool Succeeded, string? UserId, List<string> Errors)> CreateAsync(AppUser user, string password);
        Task<(bool Succeeded, List<string> Errors)> UpdateAsync(AppUser user);
        Task<(bool Succeeded, List<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        //Task<Result<bool>> ResetPasswordAsync(string email, string token, string newPassword);
        Task<AppUser?> FindByEmailAsync(string email);
        Task<AppUser?> FindByIdAsync(string id);
        Task<string[]> GetRolesAsync(AppUser user);
        Task<bool> CheckPasswordAsync(AppUser user, string password);
        Task<string> GeneratePasswordResetTokenAsync(AppUser user);
        Task<(bool Succeeded, List<string> Errors)> ResetPasswordAsync(AppUser user, string token, string newPassword);
        //Task<bool> IsInRoleAsync(AppUser user, string role);
        Task<(bool Succeeded, List<string> Errors)> AddToRoleAsync(AppUser user, string roleName);
        Task<(bool Succeeded, List<string> Errors)> RemoveFromRoleAsync(AppUser user, string roleName);
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<bool> IsUserInRoleAsync(AppUser user, string role);
        //Task<List<UserWithRoleDto>> GetAllUsersNonAdminAsync();
    }
}
