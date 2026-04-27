namespace InkWell.Category.DTOs
{
    // data angular sends when creating a category
    public class CreateCategoryDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // optional - set this if creating a child category
        public int? ParentCategoryId { get; set; }
    }
}