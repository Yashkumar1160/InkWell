using InkWell.Comment.DTOs;
using InkWell.Comment.Models;
using InkWell.Comment.Repository.Interfaces;
using InkWell.Comment.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using MassTransit;
using InkWell.Shared.Events;

namespace InkWell.Comment.Services.Services
{
    public class CommentServiceImpl : ICommentService
    {
        private ICommentRepository commentRepository;
        
        private static bool moderationModeEnabled = false;
        
        private IDistributedCache cache;
        
        private IPublishEndpoint publishEndpoint;

        // Constructor dependency Injection
        public CommentServiceImpl(ICommentRepository repository, IDistributedCache cacheService, IPublishEndpoint publish)
        {
            commentRepository = repository;
            cache = cacheService;
            publishEndpoint = publish;
        }

        // when enabled all new comments go to PENDING status
        public void SetModerationMode(bool enabled)
        {
            moderationModeEnabled = enabled;
        }

        public bool GetModerationMode()
        {
            return moderationModeEnabled;
        }


        // Method to add comment (both reply and top level comment)
        public async Task<CommentResponseDTO> AddComment(int authorId, string actorName, CreateCommentDTO dto)
        {
            int parentCommentAuthorId = 0;
            // if this is a reply check if parent comment exists
            if (dto.ParentCommentId != null)
            {
                // get parent comment
                CommentModel parent = await commentRepository.GetById(dto.ParentCommentId.Value);

                if (parent == null)
                {
                    throw new Exception("Parent comment not found.");
                }

                // if parent itself is a reply do not allow reply to reply
                if (parent.ParentCommentId != null)
                {
                    throw new Exception("You can only reply to top level comments.");
                }

                parentCommentAuthorId = parent.AuthorId;
            }

            int postAuthorId = dto.PostAuthorId;

            // Create new comment
            CommentModel newComment = new CommentModel
            {
                PostId = dto.PostId,
                AuthorId = authorId,
                ParentCommentId = dto.ParentCommentId,
                Content = dto.Content,
                LikesCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // if moderation mode is on new comments go to PENDING
            // otherwise they are APPROVED immediately
            if (moderationModeEnabled == true)
            {
                newComment.Status = "PENDING";
            }
            else
            {
                newComment.Status = "APPROVED";
            }

            CommentModel saved = await commentRepository.Add(newComment);
            
            // Invalidate cache for this post (Aggressive)
            try { await cache.RemoveAsync($"comments_post_{dto.PostId}"); } catch { /* Redis down */ }
            try { await cache.RemoveAsync($"comments_post_{saved.PostId}"); } catch { /* Redis down */ }

            // call notification service to alert the post author
            await NotifyNotificationService(saved, authorId, actorName, postAuthorId, parentCommentAuthorId);

            return MapToDTO(saved);
        }

        // Method to get all comments on a post 
        public async Task<List<CommentResponseDTO>> GetByPost(int postId)
        {
            string cacheKey = $"comments_post_{postId}";
            string cachedData = null;
            try { cachedData = await cache.GetStringAsync(cacheKey); } catch { /* Redis down */ }

            if (!string.IsNullOrEmpty(cachedData))
            {
                try { return JsonSerializer.Deserialize<List<CommentResponseDTO>>(cachedData); } catch { /* Corrupt cache */ }
            }

            // get comments on post using post id 
            List<CommentModel> comments = await commentRepository.GetByPostId(postId);

            // list to store comments
            List<CommentResponseDTO> result = new List<CommentResponseDTO>();

            foreach (CommentModel comment in comments)
            {
                result.Add(MapToDTO(comment));
            }

            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };
            try { await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), cacheOptions); } catch { /* Redis down */ }

            return result;
        }

        // Method to get only top level comments on a post
        public async Task<List<CommentResponseDTO>> GetTopLevel(int postId)
        {
            // get all top level comments
            List<CommentModel> comments = await commentRepository.GetTopLevelByPostId(postId);

            // list to store comments
            List<CommentResponseDTO> result = new List<CommentResponseDTO>();

            foreach (CommentModel comment in comments)
            {
                result.Add(MapToDTO(comment));
            }

            return result;
        }

        // Method to get all replies to a specific comment
        public async Task<List<CommentResponseDTO>> GetReplies(int parentCommentId)
        {
            // Get replies by parent id
            List<CommentModel> replies = await commentRepository.GetRepliesByParentId(parentCommentId);
            List<CommentResponseDTO> result = new List<CommentResponseDTO>();

            foreach (CommentModel comment in replies)
            {
                result.Add(MapToDTO(comment));
            }

            return result;
        }

        // Method to get comment by id
        public async Task<CommentResponseDTO> GetById(int id)
        {
            // get comment by id
            CommentModel comment = await commentRepository.GetById(id);

            if (comment == null)
            {
                throw new Exception("Comment not found.");
            }

            return MapToDTO(comment);
        }

        // Method to edit comment content
        public async Task<CommentResponseDTO> UpdateComment(int commentId, int authorId, UpdateCommentDTO dto)
        {
            CommentModel comment = await commentRepository.GetById(commentId);
            if (comment == null)
            {
                throw new Exception("Comment not found.");
            }

            // only the person who wrote the comment can edit it
            if (comment.AuthorId != authorId)
            {
                throw new Exception("You can only edit your own comments.");
            }

            // cannot edit a deleted comment
            if (comment.Status == "DELETED")
            {
                throw new Exception("Cannot edit a deleted comment.");
            }

            comment.Content = dto.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            CommentModel updated = await commentRepository.Update(comment);

            // Invalidate cache
            try { await cache.RemoveAsync($"comments_post_{comment.PostId}"); } catch { /* Redis down */ }

            return MapToDTO(updated);
        }


        // Method to soft delete a comment (set status to DELETED)
        public async Task SoftDeleteComment(int commentId, int authorId, string callerRole)
        {
            // Get comment by id 
            CommentModel comment = await commentRepository.GetById(commentId);

            if (comment == null)
            {
                throw new Exception("Comment not found.");
            }

            // check role (ADMIN or User)
            if (callerRole != "ADMIN" && comment.AuthorId != authorId)
            {
                throw new Exception("You can only delete your own comments.");
            }

            // Update status (Soft Delete)
            comment.Status = "DELETED";
            comment.UpdatedAt = DateTime.UtcNow;

            await commentRepository.Update(comment);

            // if this is a top level comment (delete replies also)
            if (comment.ParentCommentId == null)
            {
                // get replies
                List<CommentModel> replies = await commentRepository.GetRepliesByParentId(commentId);

                foreach (CommentModel reply in replies)
                {
                    // update status
                    reply.Status = "DELETED";
                    reply.UpdatedAt = DateTime.UtcNow;
                    await commentRepository.Update(reply);
                }
            }

            // Invalidate cache
            try { await cache.RemoveAsync($"comments_post_{comment.PostId}"); } catch { /* Redis down */ }
        }

        // Method to approve a pending comment
        public async Task ApproveComment(int commentId)
        {
            // Get comment by id
            CommentModel comment = await commentRepository.GetById(commentId);

            if (comment == null)
            {
                throw new Exception("Comment not found.");
            }

            // update status
            comment.Status = "APPROVED";
            comment.UpdatedAt = DateTime.UtcNow;

            await commentRepository.Update(comment);
        }

        // Method to reject a comment
        public async Task RejectComment(int commentId)
        {
            // get comment by id 
            CommentModel comment = await commentRepository.GetById(commentId);

            if (comment == null)
            {
                throw new Exception("Comment not found.");
            }

            // update status
            comment.Status = "REJECTED";
            comment.UpdatedAt = DateTime.UtcNow;

            await commentRepository.Update(comment);
        }

        // Method to add one like to comment
        public async Task LikeComment(int commentId)
        {
            // Get comment by id 
            CommentModel comment = await commentRepository.GetById(commentId);

            if (comment == null)
            {
                throw new Exception("Comment not found.");
            }

            // increase likes
            comment.LikesCount = comment.LikesCount + 1;

            await commentRepository.Update(comment);
        }

        // Method to remove one like from comment
        public async Task UnlikeComment(int commentId)
        {
            // get comment 
            CommentModel comment = await commentRepository.GetById(commentId);

            if (comment == null)
            {
                throw new Exception("Comment not found.");
            }

            // check if likes is less than 0 
            if (comment.LikesCount > 0)
            {
                comment.LikesCount = comment.LikesCount - 1;

                // update comment 
                await commentRepository.Update(comment);
            }
        }

        // Method to count approved comments on a post
        public async Task<int> GetCommentCount(int postId)
        {
            // Count comments on post 
            int count = await commentRepository.CountByPostId(postId);

            return count;
        }

        // Method to get all comments by status (admin use)
        public async Task<List<CommentResponseDTO>> GetByStatus(string status)
        {
            // validate status
            if (status != "APPROVED" && status != "PENDING"
                && status != "REJECTED" && status != "DELETED")
            {
                throw new Exception("Status must be APPROVED, PENDING, REJECTED or DELETED.");
            }

            // Get comments by status
            List<CommentModel> comments = await commentRepository.GetByStatus(status);

            // list to store comments 
            List<CommentResponseDTO> result = new List<CommentResponseDTO>();

            foreach (CommentModel comment in comments)
            {
                result.Add(MapToDTO(comment));
            }

            return result;
        }

        // Method to notify Notification Service via RabbitMQ
        private async Task NotifyNotificationService(CommentModel comment, int commentAuthorId, string actorName, int postAuthorId, int? parentCommentAuthorId)
        {
            try
            {
                string type = comment.ParentCommentId == null ? "NEW_COMMENT" : "COMMENT_REPLY";

                // Publish event to RabbitMQ
                await publishEndpoint.Publish(new CommentAddedEvent
                {
                    PostId = comment.PostId,
                    CommentId = comment.CommentId,
                    CommentAuthorId = commentAuthorId,
                    PostAuthorId = postAuthorId,
                    ParentCommentId = comment.ParentCommentId,
                    ParentCommentAuthorId = parentCommentAuthorId ?? 0,
                    NotificationType = type,
                    ActorName = actorName
                });
            }
            catch (Exception ex)
            {
                // log error but don't fail the request
                Console.WriteLine($"RabbitMQ Error: {ex.Message}");
            }
        }

        // Method to convert Comment model to CommentResponseDTO
        private CommentResponseDTO MapToDTO(CommentModel comment)
        {
            // Create comment response DTO
            CommentResponseDTO dto = new CommentResponseDTO
            {
                CommentId = comment.CommentId,
                PostId = comment.PostId,
                AuthorId = comment.AuthorId,
                ParentCommentId = comment.ParentCommentId,
                Content = comment.Content,
                LikesCount = comment.LikesCount,
                Status = comment.Status,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };

            return dto;
        }
    }
}