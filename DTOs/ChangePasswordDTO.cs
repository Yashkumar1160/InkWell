using System.ComponentModel.DataAnnotations;

namespace InkWell.Auth.DTOs
{
    // Data that Angular sends when user wants to change password
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string OldPassword{get;set;}

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "New password must contain at least one uppercase letter, one lowercase letter, and one number.")]
        public string NewPassword{get;set;}
    }
}