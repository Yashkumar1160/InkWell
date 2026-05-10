using System.ComponentModel.DataAnnotations;

namespace InkWell.Comment.Models
{
    public class CommentModel
    {
        // Primary key
        [Key]
        public int CommentId { get; set; }

        // which post this comment belongs to
        public int PostId { get; set; }

        // who wrote this comment
        public int AuthorId { get; set; }

        // if null this is a top level comment
        // if set this is a reply to another comment
        public int? ParentCommentId { get; set; }

        // the comment
        public string Content { get; set; }

        // comment likes
        public int LikesCount { get; set; }

        // Comment status (APPROVED, PENDING, REJECTED, DELETED)
        public string Status { get; set; }

        // Comment creation datetime
        public DateTime CreatedAt { get; set; }

        // when the comment was last edited
        public DateTime UpdatedAt { get; set; }


        // Constructor to set default values
        public CommentModel()
        {
            Content = "";
            LikesCount = 0;
            Status = "APPROVED";
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}