using CarShop.Application.DTOs.Identity;

namespace CarShop.Application.Interfaces
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterDto model);
        Task<string> LoginAsync(LoginDto model);
        Task LogoutAsync();
        Task<bool> CheckPasswordAsync(ApplicationUserDto userDto, string password);
        Task<bool> EmailExistsAsync(string email);
        Task<ApplicationUserDto?> GetUserByIdAsync(string userId);
        Task<ApplicationUserDto?> GetUserByEmailAsync(string email);
        Task<EditProfileDto> GetProfileAsync(string userId);
        Task UpdateProfileAsync(string userId, EditProfileDto model);
        Task ChangePasswordAsync(string userId, ChangePasswordDto model);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task ResetPasswordAsync(string email, string token, string newPassword);
        Task<List<UserWithRoleDto>> GetAllUsersWithRolesAsync();
        Task AssignRoleToUserAsync(string userId, string roleName);
        Task<List<string>> GetAllRolesAsync();
        Task<List<string>> GetAllRolesNonAdminAsync();
        Task<List<UserWithRoleDto>> GetAllUsersNonAdminAsync();
        Task CreateRoleAsync(string roleName);
    }
}
