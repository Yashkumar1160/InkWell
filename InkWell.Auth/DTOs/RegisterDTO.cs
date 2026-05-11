using System.ComponentModel.DataAnnotations;

namespace InkWell.Auth.DTOs
{
    // Data that Angular sends when user wants to create account 
    public class RegisterDTO
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        [StringLength(50, MinimumLength = 3)]
        public string Username{get;set;}

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email{get;set;}

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; }
    }
}