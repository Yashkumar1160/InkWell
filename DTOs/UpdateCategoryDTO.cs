using System.ComponentModel.DataAnnotations;

namespace InkWell.Category.DTOs
{
    // data angular sends when updating a category
    public class UpdateCategoryDTO
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}