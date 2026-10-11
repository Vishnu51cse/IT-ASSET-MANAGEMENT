using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }

    public class RegisterUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Employee"; // Admin or Employee

        public int? EmployeeId { get; set; }
    }
}
