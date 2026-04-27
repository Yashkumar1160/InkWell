using InkWell.Category.Models;

namespace InkWell.Category.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        // --- Category methods ---

        // Method to get category by id
        Task<CategoryModel> GetCategoryById(int id);

        // Method to get category by slug
        Task<CategoryModel> GetCategoryBySlug(string slug);

        // Method to get all root categories (no parent)
        Task<List<CategoryModel>> GetAllCategories();

        // Method to get child categories of a parent
        Task<List<CategoryModel>> GetChildCategories(int parentId);

        // Method to check if category slug exists
        Task<bool> CategorySlugExists(string slug);

        // Method to add new category
        Task<CategoryModel> AddCategory(CategoryModel category);

        // Method to update category
        Task<CategoryModel> UpdateCategory(CategoryModel category);

        // Method to delete category
        Task DeleteCategory(int id);
        

        // --- Tag methods ---

        // Method to get tag by id
        Task<Tag> GetTagById(int id);

        // Method to get tag by slug
        Task<Tag> GetTagBySlug(string slug);

        // Method to get all tags
        Task<List<Tag>> GetAllTags();

        // Method to get trending tags ordered by PostCount
        Task<List<Tag>> GetTrendingTags(int count);

        // Method to check if tag slug exists
        Task<bool> TagSlugExists(string slug);

        // Method to save new tag
        Task<Tag> AddTag(Tag tag);

        // Method to delete tag
        Task DeleteTag(int id);


        // --- PostTag methods ---

        // Method to get all tags assigned to a post
        Task<List<Tag>> GetTagsByPostId(int postId);

        // Method to check if post already has this tag
        Task<bool> PostTagExists(int postId, int tagId);

        // Method to assign tag to post
        Task AddPostTag(PostTag postTag);

        // Method to remove tag from post
        Task RemovePostTag(int postId, int tagId);

        // Method to update existing tag (used to increment or decrement PostCount)
        Task<Tag> UpdateTag(Tag tag);

        // Method to get all categories assigned to a specific post
        Task<List<CategoryModel>> GetCategoriesByPostId(int postId);

        // Method to check if post already has this category assigned
        Task<bool> PostCategoryExists(int postId, int categoryId);

        // Method to assign category to post
        Task AddPostCategory(PostCategory postCategory);

        // Method to remove category from post
        Task RemovePostCategory(int postId, int categoryId);
    }
}