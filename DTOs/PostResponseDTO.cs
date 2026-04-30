namespace InkWell.Post.DTOs
{
    // Data that we send back to angular
    public class PostResponseDTO
    {
        public int PostId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string Status { get; set; }
        public int ReadTimeMinutes { get; set; }
        public int ViewCount { get; set; }
        public int LikesCount { get; set; }
        public bool IsFeatured{get;set;}
        public bool IsLiked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}