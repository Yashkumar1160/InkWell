namespace InkWell.Shared.Events
{
    public class PostPublishedEvent
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int AuthorId { get; set; }
    }
}
