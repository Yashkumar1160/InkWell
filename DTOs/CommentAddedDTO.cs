namespace InkWell.Notification.DTOs
{
    // data comment service sends when comment is added
    public class CommentAddedDTO
    {
        public int PostId { get; set; }
        public int CommentId { get; set; }
        public int CommentAuthorId { get; set; }
        public int PostAuthorId { get; set; }

        // only set when this is a reply
        public int? ParentCommentId { get; set; }

        // id of the person who wrote the parent comment
        // only set when this is a reply
        public int ParentCommentAuthorId { get; set; }

        // NEW_COMMENT or COMMENT_REPLY
        public string NotificationType { get; set; }
    }

}