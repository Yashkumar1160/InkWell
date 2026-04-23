using InkWell.Comment.DTOs;

namespace InkWell.Comment.Services.Interfaces
{
    public interface ICommentService
    {
        // Method to add a new comment or reply to a post
        Task<CommentResponseDTO> AddComment(int authorId, CreateCommentDTO dto);

        // Method to get all comments on a post 
        Task<List<CommentResponseDTO>> GetByPost(int postId);

        // Method to get only top level comments on a post
        Task<List<CommentResponseDTO>> GetTopLevel(int postId);

        // Method to get all replies to a specific comment
        Task<List<CommentResponseDTO>> GetReplies(int parentCommentId);

        // Method to get comment by id
        Task<CommentResponseDTO> GetById(int id);

        // Method to edit comment content
        Task<CommentResponseDTO> UpdateComment(int commentId, int authorId, UpdateCommentDTO dto);

        // Method to soft delete a comment (set status to DELETED)
        Task SoftDeleteComment(int commentId, int authorId, string callerRole);

        // Method to approve a pending comment
        Task ApproveComment(int commentId);

        // Method to reject a comment
        Task RejectComment(int commentId);

        // Method to add one like to comment
        Task LikeComment(int commentId);

        // Method to remove one like from comment
        Task UnlikeComment(int commentId);

        // Method to count approved comments on a post
        Task<int> GetCommentCount(int postId);

        // Method to get all comments by status (admin use)
        Task<List<CommentResponseDTO>> GetByStatus(string status);

        // admin toggles platform moderation mode on or off
        void SetModerationMode(bool enabled);

        // get current moderation mode status
        bool GetModerationMode();
    }
}