using System.ComponentModel.DataAnnotations;

namespace InkWell.Category.DTOs
{
    // data angular sends when creating a tag
    public class CreateTagDTO
    {
        [Required(ErrorMessage = "Tag name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tag name must be between 2 and 50 characters.")]
        public string Name { get; set; }
    }
}