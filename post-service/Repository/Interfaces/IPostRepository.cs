using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InkWell.Post.Models;
using InkWell.Post.Models;

namespace InkWell.Post.Repository.Interfaces
{
    public interface IPostRepository
    {
        // Get post by id 
        Task<PostModel> GetById(int id);

        // Get post by slug (used for public blog urls)
        Task<PostModel> GetBySlug(string slug);

        // Get all posts by a specific author
        Task<List<PostModel>> GetByAuthorId(int authorId);

        // Get all published posts
        Task<List<PostModel>> GetPublished();

        // Search posts by keyword in title
        Task<List<PostModel>> SearchByTitle(string keyword);

        // Get all posts (admin use)
        Task<List<PostModel>> GetAll();

        // Get posts filtered by status
        Task<List<PostModel>> GetByStatus(string status);

        // Count total posts by author
        Task<int> CountByAuthorId(int authorId);

        // Save new post to database
        Task<PostModel> Add(PostModel post);

        // Save changes to existing post
        Task<PostModel> Update(PostModel post);

        // Remove post from database
        Task Delete(int id);

        // Check if slug already exists
        Task<bool> SlugExists(string slug);

        Task<LikeModel> GetLike(int postId, int userId);
        Task AddLike(LikeModel like);
        Task RemoveLike(int postId, int userId);
    }
}