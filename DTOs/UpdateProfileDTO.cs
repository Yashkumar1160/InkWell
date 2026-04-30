using System.ComponentModel.DataAnnotations;

namespace InkWell.Auth.DTOs
{
    // Data that Angular sends when user wants to update their profile
    public class UpdateProfileDTO
    {
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; }

        [MaxLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        public string Bio {get;set;}

        [Url(ErrorMessage = "Invalid URL format for Avatar.")]
        public string  AvatarUrl { get; set; }
    }
}