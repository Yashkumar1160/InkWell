using InkWell.Category.Context;
using InkWell.Category.Models;
using InkWell.Category.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace InkWell.Category.Repository.Repositories
{
    public class CategoryRepositoryImpl : ICategoryRepository
    {
        // CategoryDbContext instance
        private CategoryDbContext dbContext;

        // Constructor Dependency Injection
        public CategoryRepositoryImpl(CategoryDbContext context)
        {
            dbContext = context;
        }

        // --- Category methods ---


        // Method to get category by id
        public async Task<CategoryModel> GetCategoryById(int id)
        {
            // find gategory by id in database
            CategoryModel category = await dbContext.Categories.FindAsync(id);
            
            return category;
        }

        // Method to get category by slug
        public async Task<CategoryModel> GetCategoryBySlug(string slug)
        {
            // find category by slug in database
            CategoryModel category = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Slug == slug);
            
            return category;
        }

        // Method to get all categories ordered by name
        public async Task<List<CategoryModel>> GetAllCategories()
        {
            // get all categories from database (ordered by name)
            List<CategoryModel> categories = await dbContext.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
            
            return categories;
        }

        // Method to get children of a specific parent category
        public async Task<List<CategoryModel>> GetChildCategories(int parentId)
        {
            // get all categories where parentid matches
            List<CategoryModel> children = await dbContext.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .OrderBy(c => c.Name)
                .ToListAsync();
            
            return children;
        }

        // Method to check if category slug exists
        public async Task<bool> CategorySlugExists(string slug)
        {
            // find category in the database where slug matches
            bool exists = await dbContext.Categories.AnyAsync(c => c.Slug == slug);
            
            return exists;
        }

        // Method to add new category
        public async Task<CategoryModel> AddCategory(CategoryModel category)
        {
            // add category to dataabase
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();
            
            return category;
        }

        // Method to update category
        public async Task<CategoryModel> UpdateCategory(CategoryModel category)
        {
            // update category in the database
            dbContext.Categories.Update(category);
            await dbContext.SaveChangesAsync();
            
            return category;
        }

        // Method to delete category
        public async Task DeleteCategory(int id)
        {
            // find category from the database
            CategoryModel category = await dbContext.Categories.FindAsync(id);
            
            if (category != null)
            {   
                // remove category 
                dbContext.Categories.Remove(category);
                await dbContext.SaveChangesAsync();
            }
        }



        // --- Tag methods ---


        // Method to get tag by id
        public async Task<Tag> GetTagById(int id)
        {
            // find tag by id in database
            Tag tag = await dbContext.Tags.FindAsync(id);
            
            return tag;
        }

        // Method to get tag by slug
        public async Task<Tag> GetTagBySlug(string slug)
        {
            // find tag by slug from database   
            Tag tag = await dbContext.Tags
                .FirstOrDefaultAsync(t => t.Slug == slug);
            
            return tag;
        }

        // Method to get all tags ordered by name alphabetically
        public async Task<List<Tag>> GetAllTags()
        {
            List<Tag> tags = await dbContext.Tags
                .OrderBy(t => t.Name)
                .ToListAsync();
            
            return tags;
        }

        // Method to get trending tags ordered by PostCount
        public async Task<List<Tag>> GetTrendingTags(int count)
        {
            List<Tag> tags = await dbContext.Tags
                .OrderByDescending(t => t.PostCount)
                .Take(count)
                .ToListAsync();
            
            return tags;
        }

        // Method to check if tag slug exists
        public async Task<bool> TagSlugExists(string slug)
        {
            bool exists = await dbContext.Tags.AnyAsync(t => t.Slug == slug);
           
            return exists;
        }

        // Method to save new tag
        public async Task<Tag> AddTag(Tag tag)
        {
            dbContext.Tags.Add(tag);
            await dbContext.SaveChangesAsync();
            
            return tag;
        }

        // Method to delete tag
        public async Task DeleteTag(int id)
        {
            Tag tag = await dbContext.Tags.FindAsync(id);
           
           // if tag exists
            if (tag != null)
            {
                dbContext.Tags.Remove(tag);
                await dbContext.SaveChangesAsync();
            }
        }



        // --- PostTag methods ---


        // Method to get all tags on a specific post
        public async Task<List<Tag>> GetTagsByPostId(int postId)
        {
            List<int> tagIds = await dbContext.PostTags
                .Where(pt => pt.PostId == postId)
                .Select(pt => pt.TagId)
                .ToListAsync();

            List<Tag> tags = await dbContext.Tags
                .Where(t => tagIds.Contains(t.TagId))
                .ToListAsync();

            return tags;
        }

        // Method to check if a post already has this tag assigned
        public async Task<bool> PostTagExists(int postId, int tagId)
        {
            bool exists = await dbContext.PostTags
                .AnyAsync(pt => pt.PostId == postId && pt.TagId == tagId);
            return exists;
        }

        // Method to assign a tag to a post
        public async Task AddPostTag(PostTag postTag)
        {
            dbContext.PostTags.Add(postTag);
            await dbContext.SaveChangesAsync();
        }

        // Method to remove a tag from a post
        public async Task RemovePostTag(int postId, int tagId)
        {
            PostTag postTag = await dbContext.PostTags
                .FirstOrDefaultAsync(pt => pt.PostId == postId && pt.TagId == tagId);

            if (postTag != null)
            {
                dbContext.PostTags.Remove(postTag);
                await dbContext.SaveChangesAsync();
            }
        }

        // Method to update a tag
        public async Task<Tag> UpdateTag(Tag tag)
        {
            dbContext.Tags.Update(tag);
            await dbContext.SaveChangesAsync();
            return tag;
        }


        // Method to get all categories on a specific post
        public async Task<List<CategoryModel>> GetCategoriesByPostId(int postId)
        {
            // get all category ids for this post
            List<int> categoryIds = await dbContext.PostCategories
                .Where(pc => pc.PostId == postId)
                .Select(pc => pc.CategoryId)
                .ToListAsync();

            // get the full category objects
            List<CategoryModel> categories = await dbContext.Categories
                .Where(c => categoryIds.Contains(c.CategoryId))
                .ToListAsync();

            return categories;
        }

        // Method to check if post already has this category
        public async Task<bool> PostCategoryExists(int postId, int categoryId)
        {
            bool exists = await dbContext.PostCategories
                .AnyAsync(pc => pc.PostId == postId && pc.CategoryId == categoryId);
            return exists;
        }

        // Method to assign category to post
        public async Task AddPostCategory(PostCategory postCategory)
        {
            dbContext.PostCategories.Add(postCategory);
            await dbContext.SaveChangesAsync();
        }

        // Method to remove category from post
        public async Task RemovePostCategory(int postId, int categoryId)
        {
            PostCategory postCategory = await dbContext.PostCategories
                .FirstOrDefaultAsync(pc => pc.PostId == postId && pc.CategoryId == categoryId);

            if (postCategory != null)
            {
                dbContext.PostCategories.Remove(postCategory);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}