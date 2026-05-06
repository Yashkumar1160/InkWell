namespace InkWell.Shared.Events
{
    public class PostLikedEvent
    {
        public int PostId { get; set; }
        public int PostAuthorId { get; set; }
        public int ActorId { get; set; }
        public string ActorName { get; set; }
    }
}
