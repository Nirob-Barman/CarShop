
using System.ComponentModel.DataAnnotations;

namespace CarShop.Application.DTOs.Identity
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MinLength(5, ErrorMessage = "Email must be at least 5 characters.")]
        public string? Email { get; set; }
    }

}
