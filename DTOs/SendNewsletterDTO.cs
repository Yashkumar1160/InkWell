using System.ComponentModel.DataAnnotations;

namespace InkWell.Newsletter.DTOs
{
    // data angular sends when sending a newsletter campaign
    public class SendNewsletterDTO
    {
        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Subject must be between 3 and 200 characters.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Body is required.")]
        [MinLength(10, ErrorMessage = "Newsletter body must be at least 10 characters.")]
        public string Body { get; set; }

        // if set only subscribers with this preference get the email
        // if empty all active subscribers get it
        public string PreferenceFilter { get; set; }
    }

}