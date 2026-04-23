using InkWell.Comment.Context;
using InkWell.Comment.Models;
using InkWell.Comment.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace InkWell.Comment.Repository.Repositories
{
    public class CommentRepositoryImpl : ICommentRepository
    {
        // CommentDbContext instance
        private CommentDbContext dbContext;

        // Constructor Dependency Injection
        public CommentRepositoryImpl(CommentDbContext context)
        {
            dbContext = context;
        }

        // Method to get comment by id
        public async Task<CommentModel> GetById(int id)
        {
            // get comment from database by id
            CommentModel comment = await dbContext.Comments.FindAsync(id);
            
            return comment;
        }

        // Method to get all comments on a post (old comments first)
        public async Task<List<CommentModel>> GetByPostId(int postId)
        {
            // Get comments from database 
            List<CommentModel> comments = await dbContext.Comments
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
            
            return comments;
        }

        // Method to get only top level comments
        public async Task<List<CommentModel>> GetTopLevelByPostId(int postId)
        {
            // Get top level comments from database (ParentCommentId==null)
            List<CommentModel> comments = await dbContext.Comments
                .Where(c => c.PostId == postId && c.ParentCommentId == null)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
            
            return comments;
        }

        // Method to get all replies to a comment
        public async Task<List<CommentModel>> GetRepliesByParentId(int parentCommentId)
        {
            // Get comments from database (ParentCommentId!=null)
            List<CommentModel> replies = await dbContext.Comments
                .Where(c => c.ParentCommentId == parentCommentId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
    
            return replies;
        }

        // Method to get all comments by a user
        public async Task<List<CommentModel>> GetByAuthorId(int authorId)
        {
            // get comments from database where authorId matches
            List<CommentModel> comments = await dbContext.Comments
                .Where(c => c.AuthorId == authorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
           
            return comments;
        }

        // Method to get comments by status
        public async Task<List<CommentModel>> GetByStatus(string status)
        {
            // Get comments by status from database
            List<CommentModel> comments = await dbContext.Comments
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            
            return comments;
        }

        // Method to count total approved comments on a post
        public async Task<int> CountByPostId(int postId)
        {
            // count total comments where status is APPROVED
            int count = await dbContext.Comments.CountAsync(c => c.PostId == postId && c.Status == "APPROVED");
            
            return count;
        }

        // Method to add new comment
        public async Task<CommentModel> Add(CommentModel comment)
        {
            // add comment to database
            dbContext.Comments.Add(comment);

            // save changes
            await dbContext.SaveChangesAsync();
           
            return comment;
        }

        // Method to update existing comment
        public async Task<CommentModel> Update(CommentModel comment)
        {
            // update comment
            dbContext.Comments.Update(comment);
            await dbContext.SaveChangesAsync();
           
            return comment;
        }

        // Method to permanently remove comment from database
        public async Task Delete(int id)
        {
            // find comment by id 
            CommentModel comment = await dbContext.Comments.FindAsync(id);
            
            if (comment != null)
            {
                // remove comment from database
                dbContext.Comments.Remove(comment);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}