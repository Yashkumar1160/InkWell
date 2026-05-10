namespace InkWell.Category.DTOs
{
    // data angular sends when assigning tags to a post
    public class AssignTagDTO
    {
        public int PostId { get; set; }
        public int TagId { get; set; }
    }
}