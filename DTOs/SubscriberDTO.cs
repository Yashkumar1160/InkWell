using System.ComponentModel.DataAnnotations;

namespace InkWell.Newsletter.DTOs
{
    // data angular sends when user subscribes
    public class SubscribeDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; }

        // set if subscriber is a registered user
        public int? UserId { get; set; }
    }
}