namespace InkWell.Category.DTOs
{
    // what we send back for a category
    public class CategoryResponseDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public int PostCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}