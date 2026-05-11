namespace InkWell.Category.Models
{
    public class PostCategory
    {
        public int PostCategoryId { get; set; }

        // which post this category is assigned to
        public int PostId { get; set; }

        // which category is assigned
        public int CategoryId { get; set; }
    }
}