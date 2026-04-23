using InkWell.Comment.Models;


namespace InkWell.Comment.Repository.Interfaces
{
    public interface ICommentRepository
    {
        // Method to get a comment by id
        Task<CommentModel> GetById(int id);

        // Method to get all comments on a post
        Task<List<CommentModel>> GetByPostId(int postId);

        // Method to get only top level comments on a post (ParentCommentId=null)
        Task<List<CommentModel>> GetTopLevelByPostId(int postId);

        // Method to get all replies to a specific comment (ParentCommentId!=null)
        Task<List<CommentModel>> GetRepliesByParentId(int parentCommentId);

        // Method to get all comments written by a user
        Task<List<CommentModel>> GetByAuthorId(int authorId); 

        // Method to get all comments by status (used by admin)
        Task<List<CommentModel>> GetByStatus(string status);

        // Method to count comments on a post
        Task<int> CountByPostId(int postId);

        // Method to Add new comment to database
        Task<CommentModel> Add(CommentModel comment);

        // Method to update a comment
        Task<CommentModel> Update(CommentModel comment);

        // Method to delete a comment row from database (hard delete)
        Task Delete(int id);
    }
}