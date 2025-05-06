using System.ComponentModel.DataAnnotations;

namespace CarShop.Application.DTOs.Identity
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Full Name is required")]
        public string? FullName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }
    }
}
