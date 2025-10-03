using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface IUserService
    {
        //Task<string> RegisterAsync(RegisterDto model);
        Task<Result<string>> RegisterAsync(RegisterDto model);
        //Task<string> LoginAsync(LoginDto model);
        Task<Result<string>> LoginAsync(LoginDto model);
        //Task LogoutAsync();
        Task<Result<string>> LogoutAsync();
        Task<bool> CheckPasswordAsync(ApplicationUserDto userDto, string password);
        Task<bool> EmailExistsAsync(string email);
        //Task<ApplicationUserDto?> GetUserByIdAsync(string userId);
        //Task<ApplicationUserDto?> GetUserByEmailAsync(string email);
        //Task<EditProfileDto> GetProfileAsync(string userId);
        Task<Result<EditProfileDto>> GetProfileAsync(string userId);
        //Task UpdateProfileAsync(string userId, EditProfileDto model);
        Task<Result<bool>> UpdateProfileAsync(string userId, EditProfileDto model);
        //Task ChangePasswordAsync(string userId, ChangePasswordDto model);
        Task<Result<bool>> ChangePasswordAsync(string userId, ChangePasswordDto model);
        //Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<Result<string>> GeneratePasswordResetTokenAsync(string email);
        //Task ResetPasswordAsync(string email, string token, string newPassword);
        Task<Result<bool>> ResetPasswordAsync(string email, string token, string newPassword);
        //Task<List<UserWithRoleDto>> GetAllUsersWithRolesAsync();
        //Task<List<UserWithRoleDto>> GetAllUsersNonAdminAsync();
        Task<Result<List<UserWithRoleDto>>> GetAllUsersNonAdminAsync();
        //Task<List<string>> GetAllRolesNonAdminAsync();
        Task<Result<List<string>>> GetAllRolesNonAdminAsync();
        //Task AssignRoleToUserAsync(string userId, string roleName);
        Task<Result<bool>> AssignRoleToUserAsync(string userId, string roleName);
        //Task<List<string>> GetAllRolesAsync();
        //Task CreateRoleAsync(string roleName);
        Task<Result<bool>> CreateRoleAsync(string roleName);
    }
}
