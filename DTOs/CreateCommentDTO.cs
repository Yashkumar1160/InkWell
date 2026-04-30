using System.ComponentModel.DataAnnotations;

namespace InkWell.Comment.DTOs
{
    // data angular sends when adding a new comment
    public class CreateCommentDTO
    {
        [Required(ErrorMessage = "PostId is required.")]
        public int PostId { get; set; }

        // if replying to a comment set this
        // if null it is a top level comment
        public int? ParentCommentId { get; set; }
        
        [Required(ErrorMessage = "Comment content is required.")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 2000 characters.")]
        public string Content { get; set; }

        // Angular sends this so we know who owns the post
        // Angular reads it from the post detail page data
        public int PostAuthorId { get; set; }
    }
}