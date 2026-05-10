namespace InkWell.Shared.Events
{
    public class CommentAddedEvent
    {
        public int PostId { get; set; }
        public int CommentId { get; set; }
        public int CommentAuthorId { get; set; }
        public int PostAuthorId { get; set; }
        public int? ParentCommentId { get; set; }
        public int ParentCommentAuthorId { get; set; }
        public string NotificationType { get; set; }
        public string ActorName { get; set; }
        public string PostTitle { get; set; }
    }
}
