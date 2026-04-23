namespace InkWell.Comment.DTOs
{
    // data angular sends when adding a new comment
    public class CreateCommentDTO
    {
        public int PostId { get; set; }

        // if replying to a comment set this
        // if null it is a top level comment
        public int? ParentCommentId { get; set; }
        
        public string Content { get; set; }
    }
}