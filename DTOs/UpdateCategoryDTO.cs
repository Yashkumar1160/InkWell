namespace InkWell.Category.DTOs
{
    // data angular sends when updating a category
    public class UpdateCategoryDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}