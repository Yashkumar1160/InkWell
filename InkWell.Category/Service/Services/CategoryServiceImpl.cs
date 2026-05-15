using InkWell.Category.DTOs;
using InkWell.Category.Models;
using InkWell.Category.Repository.Interfaces;
using InkWell.Category.Service.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;


namespace InkWell.Category.Service.Services
{
    public class CategoryServiceImpl : ICategoryService
    {
        // ICategoryRepository instance
        private ICategoryRepository categoryRepository;
        
        private IDistributedCache cache;

        // Constructor Dependency Injection 
        public CategoryServiceImpl(ICategoryRepository repository, IDistributedCache cacheService)
        {
            categoryRepository = repository;
            cache = cacheService;
        }

        // Method to create new category (Admin only)
        public async Task<CategoryResponseDTO> CreateCategory(CreateCategoryDTO dto)
        {
            // generate slug from name
            string slug = GenerateSlug(dto.Name);

            // check slug is not already taken
            bool slugExists = await categoryRepository.CategorySlugExists(slug);
            
            if (slugExists == true)
            {
                throw new Exception("A category with this name already exists.");
            }

            // if parent is set check it actually exists
            if (dto.ParentCategoryId != null)
            {
                CategoryModel parent = await categoryRepository.GetCategoryById(dto.ParentCategoryId.Value);
                if (parent == null)
                {
                    throw new Exception("Parent category not found.");
                }
            }

            CategoryModel newCategory = new CategoryModel();
            newCategory.Name = dto.Name;
            newCategory.Slug = slug;
            newCategory.Description = dto.Description ?? "";
            newCategory.ParentCategoryId = dto.ParentCategoryId;
            newCategory.PostCount = 0;
            newCategory.CreatedAt = DateTime.UtcNow;

            CategoryModel saved = await categoryRepository.AddCategory(newCategory);
          
            // Invalidate cache
            try { await cache.RemoveAsync("all_categories"); } catch { /* Redis down */ }

            return MapCategoryToDTO(saved);
        }

        // Method to get category by id
        public async Task<CategoryResponseDTO> GetCategoryById(int id)
        {
            CategoryModel category = await categoryRepository.GetCategoryById(id);
           
            if (category == null)
            {
                throw new Exception("Category not found.");
            }
           
            return MapCategoryToDTO(category);
        }

        // Method to get category by slug
        public async Task<CategoryResponseDTO> GetCategoryBySlug(string slug)
        {
            CategoryModel category = await categoryRepository.GetCategoryBySlug(slug);
            
            if (category == null)
            {
                throw new Exception("Category not found.");
            }
            
            return MapCategoryToDTO(category);
        }

        // Method to get all categories
        public async Task<List<CategoryResponseDTO>> GetAllCategories()
        {
            string cacheKey = "all_categories";
            string cachedData = null;
            try { cachedData = await cache.GetStringAsync(cacheKey); } catch { /* Redis down */ }

            if (!string.IsNullOrEmpty(cachedData))
            {
                try { return JsonSerializer.Deserialize<List<CategoryResponseDTO>>(cachedData); } catch { /* Corrupt cache */ }
            }

            List<CategoryModel> categories = await categoryRepository.GetAllCategories();
            List<CategoryResponseDTO> result = new List<CategoryResponseDTO>();

            foreach (CategoryModel category in categories)
            {
                result.Add(MapCategoryToDTO(category));
            }
            
            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) };
            try { await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), cacheOptions); } catch { /* Redis down */ }

            return result;
        }

        // Method to get child categories of a parent
        public async Task<List<CategoryResponseDTO>> GetChildCategories(int parentId)
        {
            List<CategoryModel> children = await categoryRepository.GetChildCategories(parentId);
            List<CategoryResponseDTO> result = new List<CategoryResponseDTO>();

            foreach (CategoryModel category in children)
            {
                result.Add(MapCategoryToDTO(category));
            }
            
            return result;
        }

        // Method to update category (Admin only)
        public async Task<CategoryResponseDTO> UpdateCategory(int id, UpdateCategoryDTO dto)
        {
            CategoryModel category = await categoryRepository.GetCategoryById(id);
           
            if (category == null)
            {
                throw new Exception("Category not found.");
            }

            // regenerate slug if name changed
            string newSlug = GenerateSlug(dto.Name);

            // only check slug conflict if name actually changed
            if (newSlug != category.Slug)
            {
                bool slugExists = await categoryRepository.CategorySlugExists(newSlug);
                if (slugExists == true)
                {
                    throw new Exception("A category with this name already exists.");
                }
                category.Slug = newSlug;
            }

            category.Name = dto.Name;
            category.Description = dto.Description ?? "";
            category.ParentCategoryId = dto.ParentCategoryId;

            CategoryModel updated = await categoryRepository.UpdateCategory(category);
           
            // Invalidate cache
            try { await cache.RemoveAsync("all_categories"); } catch { /* Redis down */ }

            return MapCategoryToDTO(updated);
        }

        // Method to delete category (Admin only)
        public async Task DeleteCategory(int id)
        {
            CategoryModel category = await categoryRepository.GetCategoryById(id);
          
            if (category == null)
            {
                throw new Exception("Category not found.");
            }
          
            await categoryRepository.DeleteCategory(id);

            // Invalidate cache
            try { await cache.RemoveAsync("all_categories"); } catch { /* Redis down */ }
        }


        // Method to create new tag (Admin only)
        public async Task<TagResponseDTO> CreateTag(CreateTagDTO dto)
        {
            // generate slug from tag name
            string slug = GenerateSlug(dto.Name);

            // check slug is not already taken
            bool slugExists = await categoryRepository.TagSlugExists(slug);
            
            if (slugExists == true)
            {
                throw new Exception("A tag with this name already exists.");
            }

            Tag newTag = new Tag();
            newTag.Name = dto.Name;
            newTag.Slug = slug;
            newTag.PostCount = 0;
            newTag.CreatedAt = DateTime.UtcNow;

            Tag saved = await categoryRepository.AddTag(newTag);
            
            // Invalidate cache
            try { await cache.RemoveAsync("all_tags"); } catch { /* Redis down */ }

            return MapTagToDTO(saved);
        }

        // Method to get tag by id
        public async Task<TagResponseDTO> GetTagById(int id)
        {
            Tag tag = await categoryRepository.GetTagById(id);
           
            if (tag == null)
            {
                throw new Exception("Tag not found.");
            }
           
            return MapTagToDTO(tag);
        }

        // Method to get tag by slug
        public async Task<TagResponseDTO> GetTagBySlug(string slug)
        {
            Tag tag = await categoryRepository.GetTagBySlug(slug);
            
            if (tag == null)
            {
                throw new Exception("Tag not found.");
            }
            
            return MapTagToDTO(tag);
        }

        // Method to get all tags
        public async Task<List<TagResponseDTO>> GetAllTags()
        {
            string cacheKey = "all_tags";
            string cachedData = null;
            try { cachedData = await cache.GetStringAsync(cacheKey); } catch { /* Redis down */ }

            if (!string.IsNullOrEmpty(cachedData))
            {
                try { return JsonSerializer.Deserialize<List<TagResponseDTO>>(cachedData); } catch { /* Corrupt cache */ }
            }

            List<Tag> tags = await categoryRepository.GetAllTags();
            List<TagResponseDTO> result = new List<TagResponseDTO>();

            foreach (Tag tag in tags)
            {
                result.Add(MapTagToDTO(tag));
            }
          
            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) };
            try { await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), cacheOptions); } catch { /* Redis down */ }

            return result;
        }

        // Method to get top 10 trending tags by PostCount
        public async Task<List<TagResponseDTO>> GetTrendingTags()
        {
            // get top 10 trending tags
            List<Tag> tags = await categoryRepository.GetTrendingTags(10);
            List<TagResponseDTO> result = new List<TagResponseDTO>();

            foreach (Tag tag in tags)
            {
                result.Add(MapTagToDTO(tag));
            }
           
            return result;
        }

        // Method to Delete a Tag
        public async Task DeleteTag(int id)
        {
            Tag tag = await categoryRepository.GetTagById(id);
            if (tag == null)
            {
                throw new Exception("Tag not found.");
            }
           
            await categoryRepository.DeleteTag(id);

            // Invalidate cache
            try { await cache.RemoveAsync("all_tags"); } catch { /* Redis down */ }
        }

        // Method to add tag to a post
        public async Task AddTagToPost(int postId, int tagId)
        {
            // check tag exists
            Tag tag = await categoryRepository.GetTagById(tagId);
           
            if (tag == null)
            {
                throw new Exception("Tag not found.");
            }

            // check this tag is not already on this post
            bool alreadyAssigned = await categoryRepository.PostTagExists(postId, tagId);

            if (alreadyAssigned == true)
            {
                return; // Already exists - return gracefully
            }

            // create the PostTag record
            PostTag postTag = new PostTag();
            postTag.PostId = postId;
            postTag.TagId = tagId;

            await categoryRepository.AddPostTag(postTag);

            // increment tag PostCount
            tag.PostCount = tag.PostCount + 1;

            await categoryRepository.UpdateTag(tag);
        }

        // Method to remove tag from a post
        public async Task RemoveTagFromPost(int postId, int tagId)
        {
            // check this tag is actually on this post
            bool exists = await categoryRepository.PostTagExists(postId, tagId);
            
            if (exists == false)
            {
                throw new Exception("This tag is not assigned to this post.");
            }

            await categoryRepository.RemovePostTag(postId, tagId);

            // decrement tag PostCount
            Tag tag = await categoryRepository.GetTagById(tagId);
           
            if (tag != null && tag.PostCount > 0)
            {
                tag.PostCount = tag.PostCount - 1;
                await categoryRepository.UpdateTag(tag);
            }
        }

        // Method to get tags by post count
        public async Task<List<TagResponseDTO>> GetTagsByPost(int postId)
        {
            string cacheKey = $"post_tags_{postId}";
            string cachedData = null;
            try { cachedData = await cache.GetStringAsync(cacheKey); } catch { /* Redis down */ }

            if (!string.IsNullOrEmpty(cachedData))
            {
                try { return JsonSerializer.Deserialize<List<TagResponseDTO>>(cachedData); } catch { /* Corrupt cache */ }
            }

            List<Tag> tags = await categoryRepository.GetTagsByPostId(postId);
            List<TagResponseDTO> result = new List<TagResponseDTO>();

            foreach (Tag tag in tags)
            {
                result.Add(MapTagToDTO(tag));
            }

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            try { await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), cacheOptions); } catch { /* Redis down */ }
           
            return result;
        }

        // Method to generate url safe slug from name
        private string GenerateSlug(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            string slug = name.ToLower();
            slug = slug.Replace(" ", "-");

            string cleanSlug = "";
           
            foreach (char c in slug)
            {
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '-')
                {
                    cleanSlug = cleanSlug + c;
                }
            }
           
            return cleanSlug;
        }

        // Method to convert Category to CategoryResponseDTO
        private CategoryResponseDTO MapCategoryToDTO(CategoryModel category)
        {
            CategoryResponseDTO dto = new CategoryResponseDTO();
            dto.CategoryId = category.CategoryId;
            dto.Name = category.Name;
            dto.Slug = category.Slug;
            dto.Description = category.Description;
            dto.ParentCategoryId = category.ParentCategoryId;
            dto.PostCount = category.PostCount;
            dto.CreatedAt = category.CreatedAt;
           
            return dto;
        }

        // Method to convert a Tag to TagResponseDTO
        private TagResponseDTO MapTagToDTO(Tag tag)
        {
            TagResponseDTO dto = new TagResponseDTO();
            dto.TagId = tag.TagId;
            dto.Name = tag.Name;
            dto.Slug = tag.Slug;
            dto.PostCount = tag.PostCount;
            dto.CreatedAt = tag.CreatedAt;
           
            return dto;
        }

        // Method to add category to a post
        public async Task AddCategoryToPost(int postId, int categoryId)
        {
            // check category exists
            CategoryModel category = await categoryRepository.GetCategoryById(categoryId);
          
            if (category == null)
            {
                throw new Exception("Category not found.");
            }

            // check this category is not already assigned to this post
            bool alreadyAssigned = await categoryRepository.PostCategoryExists(postId, categoryId);
          
            if (alreadyAssigned == true)
            {
                return; // Already exists - return gracefully
            }

            // create the PostCategory record
            PostCategory postCategory = new PostCategory();
            postCategory.PostId = postId;
            postCategory.CategoryId = categoryId;

            await categoryRepository.AddPostCategory(postCategory);

            // increment category PostCount
            category.PostCount = category.PostCount + 1;
            await categoryRepository.UpdateCategory(category);

            // Invalidate cache for this post
            try { await cache.RemoveAsync($"post_categories_{postId}"); } catch { /* Redis down */ }
        }

        // Method to remove a tag from a post
        public async Task RemoveCategoryFromPost(int postId, int categoryId)
        {
            // check this category is actually on this post
            bool exists = await categoryRepository.PostCategoryExists(postId, categoryId);
            
            if (exists == false)
            {
                throw new Exception("This category is not assigned to this post.");
            }

            await categoryRepository.RemovePostCategory(postId, categoryId);

            // decrement category PostCount
            
            CategoryModel category = await categoryRepository.GetCategoryById(categoryId);
            
            if (category != null && category.PostCount > 0)
            {
                category.PostCount = category.PostCount - 1;
                await categoryRepository.UpdateCategory(category);
            }

            // Invalidate cache for this post
            try { await cache.RemoveAsync($"post_categories_{postId}"); } catch { /* Redis down */ }
        }

        // Method to get categories by a post
        public async Task<List<CategoryResponseDTO>> GetCategoriesByPost(int postId)
        {
            string cacheKey = $"post_categories_{postId}";
            string cachedData = null;
            try { cachedData = await cache.GetStringAsync(cacheKey); } catch { /* Redis down */ }

            if (!string.IsNullOrEmpty(cachedData))
            {
                try { return JsonSerializer.Deserialize<List<CategoryResponseDTO>>(cachedData); } catch { /* Corrupt cache */ }
            }

            List<CategoryModel> categories = await categoryRepository.GetCategoriesByPostId(postId);
            List<CategoryResponseDTO> result = new List<CategoryResponseDTO>();

            foreach (CategoryModel category in categories)
            {
                result.Add(MapCategoryToDTO(category));
            }

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            try { await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), cacheOptions); } catch { /* Redis down */ }
           
            return result;
        }

        public async Task<List<int>> GetPostIdsByCategorySlug(string slug)
        {
            var category = await categoryRepository.GetCategoryBySlug(slug);
            if (category == null) return new List<int>();
            return await categoryRepository.GetPostIdsByCategoryId(category.CategoryId);
        }

        public async Task<List<int>> GetPostIdsByTagSlug(string slug)
        {
            var tag = await categoryRepository.GetTagBySlug(slug);
            if (tag == null) return new List<int>();
            return await categoryRepository.GetPostIdsByTagId(tag.TagId);
        }
    }
}