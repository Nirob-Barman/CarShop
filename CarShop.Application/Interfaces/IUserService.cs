using CarShop.Application.DTOs.Identity;

namespace CarShop.Application.Interfaces
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterDto model);
        Task<string> LoginAsync(LoginDto model);
        Task LogoutAsync();
        Task<EditProfileDto> GetProfileAsync(string userId);
        Task UpdateProfileAsync(string userId, EditProfileDto model);
        Task ChangePasswordAsync(string userId, ChangePasswordDto model);
    }
}
