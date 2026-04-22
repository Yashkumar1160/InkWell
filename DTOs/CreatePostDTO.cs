namespace InkWell.Post.DTOs
{
    // Data that angular sends when creating a new post
    public class CreatePostDTO
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public string FeaturedImageUrl { get; set; }
    }
}