using System.ComponentModel.DataAnnotations;

namespace InkWell.Post.Models
{
    public class PostModel
    {
        // Primary key
        [Key]
        public int PostId { get; set; }

        // AuthorId link to UserId in auth service
        public int AuthorId { get; set; }

        // Post Title
        public string Title { get; set; }

        // URL friendly version
        public string Slug { get; set; }

        // Post Content
        public string Content { get; set; }

        // Short Summary 
        public string Excerpt { get; set; }

        // Feature Image URL
        public string FeaturedImageUrl { get; set; }

        // Status (DRAFT, PUBLISHED, UNPUBLISHED, ARCHIVED)
        public string Status { get; set; }

        // Read Time in minutes
        public int ReadTimeMinutes { get; set; }

        // Total number of views
        public int ViewCount { get; set; }

        // Total number of likes
        public int LikesCount { get; set; }

        // Admin can pin post at top of page
        public bool IsFeatured { get; set; }

        // When post was first created
        public DateTime CreatedAt { get; set; }

        // When post was last edited
        public DateTime UpdatedAt { get; set; }

        // When post was published (null if not published yet)
        public DateTime? PublishedAt { get; set; }

        // Constructor to set default values
        public PostModel()
        {
            Title = "";
            Slug = "";
            Content = "";
            Excerpt = "";
            FeaturedImageUrl = "";
            Status = "DRAFT";
            ReadTimeMinutes = 0;
            ViewCount = 0;
            LikesCount = 0;
            IsFeatured = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            PublishedAt = null;
        }
    }
}


