
namespace InkWell.Category.DTOs
{

    // what we send back for a tag
    public class TagResponseDTO
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int PostCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}