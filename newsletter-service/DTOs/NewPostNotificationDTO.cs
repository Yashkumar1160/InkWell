namespace InkWell.Newsletter.DTOs
{
    // data post service sends when a new post is published
    public class NewPostNotificationDTO
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int AuthorId { get; set; }
    }
}