using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InkWell.Post.DTOs;

namespace InkWell.Post.Service.Interfaces
{
    public interface IPostService
    {
        // Method to create new draft post
        Task<PostResponseDTO> CreatePost(int authorId, CreatePostDTO dto);

        // Method to get single post by id
        Task<PostResponseDTO> GetById(int id);

        // Method to get single post by slug 
        Task<PostResponseDTO> GetBySlug(string slug);

        // Method to get all posts by author
        Task<List<PostResponseDTO>> GetByAuthor(int authorId);

        // Method to get all published posts 
        Task<List<PostResponseDTO>> GetPublished();

        // Method to search published posts by keyword
        Task<List<PostResponseDTO>> Search(string keyword);

        // Method to get all posts (admin panel)
        Task<List<PostResponseDTO>> GetAll();

        // Method to get posts filtered by status (admin use)
        Task<List<PostResponseDTO>> GetByStatus(string status);

        // Method to get total post count for an author
        Task<int> GetPostCount(int authorId);


        // Method to edit post, title, content etc
        Task<PostResponseDTO> UpdatePost(int postId, int authorId, UpdatePostDTO dto);

        // Method to change status to PUBLISHED
        Task<PostResponseDTO> PublishPost(int postId, int authorId);

        // Method to change status to UNPUBLISHED
        Task<PostResponseDTO> UnpublishPost(int postId, int authorId);

        // Method to change status to ARCHIVED
        Task<PostResponseDTO> ArchivePost(int postId, int authorId);

        // Method to delete post 
        Task DeletePost(int postId, int authorId, string callerRole);

        // Method for admin to pin post to top of feed
        Task<PostResponseDTO> FeaturePost(int postId);

        // Method for admin to remove pin from post
        Task<PostResponseDTO> UnfeaturePost(int postId);

        // Method to add one view to the post
        Task IncrementViews(int postId);

        // Method to add one like to the post
        Task LikePost(int postId);

        // Method to remove one like from the post
        Task UnlikePost(int postId);

    }
}