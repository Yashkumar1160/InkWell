using System.ComponentModel.DataAnnotations;

namespace InkWell.Post.DTOs
{
    // Data that angular sends when creating a new post
    public class CreatePostDTO
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [MinLength(10, ErrorMessage = "Content must be at least 10 characters.")]
        public string Content { get; set; }

        [MaxLength(500, ErrorMessage = "Excerpt cannot exceed 500 characters.")]
        public string? Excerpt { get; set; }

        [Url(ErrorMessage = "Featured image must be a valid URL.")]
        public string? FeaturedImageUrl { get; set; }
    }
}