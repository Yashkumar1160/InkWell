namespace InkWell.Category.DTOs
{
    // data angular sends when assigning category to post
    public class AssignCategoryDTO
    {
        public int PostId { get; set; }
        public int CategoryId { get; set; }
    }
}