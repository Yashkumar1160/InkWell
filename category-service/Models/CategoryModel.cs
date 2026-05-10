using System.ComponentModel.DataAnnotations;

namespace InkWell.Category.Models
{
    public class CategoryModel
    {
        // primary key
        [Key]
        public int CategoryId { get; set; }

        // category  name
        public string Name { get; set; }

        // slug
        public string Slug { get; set; }

        // category description 
        public string? Description { get; set; }

        // parent categoryid == null (top level)
        // parent categoryid != null (child)
        public int? ParentCategoryId { get; set; }

        // post count
        public int PostCount { get; set; }

        // category creation
        public DateTime CreatedAt { get; set; }

        // Constructor for setting default values
        public CategoryModel()
        {
            Name = "";
            Slug = "";
            Description = "";
            ParentCategoryId = null;
            PostCount = 0;
            CreatedAt = DateTime.UtcNow;
        }
    }
}