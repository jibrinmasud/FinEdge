using System.ComponentModel.DataAnnotations;

namespace FinEdge.Application.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string? UserName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required, Compare(nameof(Password))]
        public string? ComfirmPassword { get; set; }
    }
}