using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InkWell.Post.Context;
using InkWell.Post.Models;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using InkWell.Post.Repository.Interfaces;


namespace InkWell.Post.Repository.Repositories
{
    public class PostRepositoryImpl : IPostRepository
    {
        // PostDbContext instance
        private PostDbContext dbContext;

        // Constructor Dependency Injection
        public PostRepositoryImpl(PostDbContext context)
        {
            dbContext = context;
        }

        // Method to find post by id
        public async Task<PostModel> GetById(int id)
        {
            // Find post in database with id 
            PostModel post = await dbContext.Posts.FindAsync(id);
            return post;
        }

        // Method to find post by slug - used for public blog page
        public async Task<PostModel> GetBySlug(string slug)
        {
            // Find post in database with slug
            PostModel post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
            return post;
        }

        // Method to Get all posts written by a specific author
        // ordered newest first
        public async Task<List<PostModel>> GetByAuthorId(int authorId)
        {
            // Find all posts in database with matching authorId
            List<PostModel> posts = await dbContext.Posts.Where(p => p.AuthorId == authorId)
                .OrderByDescending(p => p.CreatedAt).ToListAsync();
            return posts;
        }

        // Method to get all published posts for homepage
        public async Task<List<PostModel>> GetPublished()
        {
            // Find posts in database where status is PUBLISHED
            List<PostModel> posts = await dbContext.Posts.Where(p => p.Status == "PUBLISHED")
                .OrderByDescending(p => p.PublishedAt).ToListAsync();
            return posts;
        }

        // Method to get featured posts pinned by admin
        public async Task<List<PostModel>> GetFeatured()
        {
            List<PostModel> posts = await dbContext.Posts.Where(p => p.Status == "PUBLISHED" && p.IsFeatured == true)
                .OrderByDescending(p => p.PublishedAt).ToListAsync();
            return posts;
        }

        // Method to search posts where title contains specific keyword
        public async Task<List<PostModel>> SearchByTitle(string keyword)
        {
            // Find posts in database where status is PUBLISHED and title contains the keyword
            List<PostModel> posts = await dbContext.Posts.Where(p => p.Title.Contains(keyword) && p.Status == "PUBLISHED")
                .OrderByDescending(p => p.PublishedAt).ToListAsync();
            return posts;
        }

        // Method to get all posts regardless of status - admin use
        public async Task<List<PostModel>> GetAll()
        {
            // Find all the posts in database (recent first)
            List<PostModel> posts = await dbContext.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return posts;
        }

        // Method to get posts by specific status
        public async Task<List<PostModel>> GetByStatus(string status)
        {
            List<PostModel> posts = await dbContext.Posts.Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt).ToListAsync();
            return posts;
        }

        // Method to count how many posts an author has written
        public async Task<int> CountByAuthorId(int authorId)
        {
            int count = await dbContext.Posts
                .CountAsync(p => p.AuthorId == authorId);
            return count;
        }

        // Method to save new post
        public async Task<PostModel> Add(PostModel post)
        {
            // Add post to database
            dbContext.Posts.Add(post);

            // save changes to database
            await dbContext.SaveChangesAsync();
            return post;
        }

        // Method to save changes to existing post
        public async Task<PostModel> Update(PostModel post)
        {
            // Update post in database
            dbContext.Posts.Update(post);

            // save changes in database
            await dbContext.SaveChangesAsync();
            return post;
        }

        // Method to delete post from database
        public async Task Delete(int id)
        {
            // Find post by id 
            PostModel post = await dbContext.Posts.FindAsync(id);

            // if post is not null
            if (post != null)
            {
                // remove post from database
                dbContext.Posts.Remove(post);

                // save changes to database
                await dbContext.SaveChangesAsync();
            }
        }

        // Method to check if slug is already taken before saving
        public async Task<bool> SlugExists(string slug)
        {
            // find post by slug 
            bool exists = await dbContext.Posts.AnyAsync(p => p.Slug == slug);
            return exists;
        }
    }
}