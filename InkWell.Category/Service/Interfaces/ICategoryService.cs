using InkWell.Category.DTOs;

namespace InkWell.Category.Service.Interfaces
{
    public interface ICategoryService
    {
        // --- Category operations ---

        // Method to create new category (Admin only)
        Task<CategoryResponseDTO> CreateCategory(CreateCategoryDTO dto);

        // Method to get category by id
        Task<CategoryResponseDTO> GetCategoryById(int id);

        // Method to get category by slug
        Task<CategoryResponseDTO> GetCategoryBySlug(string slug);

        // Method to get all categories
        Task<List<CategoryResponseDTO>> GetAllCategories();

        // Method to get child categories of a parent
        Task<List<CategoryResponseDTO>> GetChildCategories(int parentId);

        // Method to update category (Admin only)
        Task<CategoryResponseDTO> UpdateCategory(int id, UpdateCategoryDTO dto);

        // Method to delete category (Admin only)
        Task DeleteCategory(int id);

        
        // --- Tag operations ---

        // Method to create new tag (Admin only)
        Task<TagResponseDTO> CreateTag(CreateTagDTO dto);

        // Method to get tag by id
        Task<TagResponseDTO> GetTagById(int id);

        // Method to get tag by slug
        Task<TagResponseDTO> GetTagBySlug(string slug);

        // Method to get all tags
        Task<List<TagResponseDTO>> GetAllTags();

        // Method to get top 10 trending tags by PostCount
        Task<List<TagResponseDTO>> GetTrendingTags();

        // Method to delete tag (Admin only)
        Task DeleteTag(int id);


        // --- PostTag operations ---

        // Method to assign a tag to a post
        Task AddTagToPost(int postId, int tagId);

        // Method to remove a tag from a post
        Task RemoveTagFromPost(int postId, int tagId);

        // Method to get all tags on a specific post
        Task<List<TagResponseDTO>> GetTagsByPost(int postId);

        // Method to assign a category to a post
        Task AddCategoryToPost(int postId, int categoryId);

        // Method to remove a category from a post
        Task RemoveCategoryFromPost(int postId, int categoryId);

        // Method to get all categories a post belongs to
        Task<List<CategoryResponseDTO>> GetCategoriesByPost(int postId);

        // Method to get all post ids for a category slug
        Task<List<int>> GetPostIdsByCategorySlug(string slug);

        // Method to get all post ids for a tag slug
        Task<List<int>> GetPostIdsByTagSlug(string slug);
    }
}