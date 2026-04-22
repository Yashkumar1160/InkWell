namespace InkWell.Post.DTOs
{
    // Data that angular sends when editing an existing post
    public class UpdatePostDTO
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public string FeaturedImageUrl { get; set; }
    }
}