using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InkWell.Notification.DTOs;
using InkWell.Notification.Models;
using InkWell.Notification.Repository.Interfaces;
using InkWell.Notification.Services.Interfaces;

namespace InkWell.Notification.Services.Services
{
    public class NotificationServiceImpl : INotificationService
    {
        // INotificationRepository instance 
        private INotificationRepository notificationRepository;

        // Constructor Dependency Injection
        public NotificationServiceImpl(INotificationRepository repository)
        {
            notificationRepository = repository;
        }

        // Method to create and save a single notification
        public async Task<NotificationResponseDTO> Send(int recipientId, int actorId, string type, string title, string message,
            int relatedId, string relatedType)
        {
            NotificationModel newNotification = new NotificationModel();
            newNotification.RecipientId = recipientId;
            newNotification.ActorId = actorId;
            newNotification.Type = type;
            newNotification.Title = title;
            newNotification.Message = message;
            newNotification.RelatedId = relatedId;
            newNotification.RelatedType = relatedType;
            newNotification.IsRead = false;
            newNotification.CreatedAt = DateTime.UtcNow;

            NotificationModel saved = await notificationRepository.Add(newNotification);
            return MapToDTO(saved);
        }

        // Method to send notification to multiple recipients at once (Admin broadcast)
        public async Task SendBulk(BroadcastDTO dto)
        {
            if (dto.RecipientIds == null || dto.RecipientIds.Count == 0)
            {
                throw new Exception("No recipients provided.");
            }

            foreach (int recipientId in dto.RecipientIds)
            {
                NotificationModel notification = new NotificationModel();
                notification.RecipientId = recipientId;
                // 0 means system sent this
                notification.ActorId = 0;      
                notification.Type = "BROADCAST";
                notification.Title = dto.Title;
                notification.Message = dto.Message;
                notification.RelatedId = 0;
                notification.RelatedType = "System";
                notification.IsRead = false;
                notification.CreatedAt = DateTime.UtcNow;

                await notificationRepository.Add(notification);
            }
        }

        // Method to get all notifications for a user
        public async Task<List<NotificationResponseDTO>> GetByRecipient(int recipientId)
        {
            List<NotificationModel> notifications = await notificationRepository.GetByRecipientId(recipientId);

            List<NotificationResponseDTO> result = new List<NotificationResponseDTO>();

            foreach (NotificationModel notification in notifications)
            {
                result.Add(MapToDTO(notification));
            }
            
            return result;
        }

        // Method to get unread notifications for a user
        public async Task<List<NotificationResponseDTO>> GetUnread(int recipientId)
        {
            List<NotificationModel> notifications = await notificationRepository
                .GetUnreadByRecipientId(recipientId);

            List<NotificationResponseDTO> result = new List<NotificationResponseDTO>();

            foreach (NotificationModel notification in notifications)
            {
                result.Add(MapToDTO(notification));
            }
           
            return result;
        }

        // Method to mark a single notification as read
        public async Task MarkAsRead(int notificationId, int recipientId)
        {
            NotificationModel notification = await notificationRepository.GetById(notificationId);

            if (notification == null)
            {
                throw new Exception("Notification not found.");
            }

            // user can only mark their own notifications as read
            if (notification.RecipientId != recipientId && notification.RecipientId != 0)
            {
                throw new Exception("You can only mark your own notifications as read.");
            }

            notification.IsRead = true;
            await notificationRepository.Update(notification);
        }

        // Method to mark all notifications as read for a user
        public async Task MarkAllRead(int recipientId)
        {
            List<NotificationModel> unread = await notificationRepository
                .GetUnreadByRecipientId(recipientId);

            foreach (NotificationModel notification in unread)
            {
                notification.IsRead = true;
                await notificationRepository.Update(notification);
            }
        }

        // Method to delete all read notifications for a user
        public async Task DeleteRead(int recipientId)
        {
            // pass true to delete only read notifications
            await notificationRepository.DeleteByRecipientIdAndIsRead(recipientId, true);
        }

        // Method to delete a single notification
        public async Task DeleteNotification(int notificationId, int recipientId)
        {
            NotificationModel notification = await notificationRepository.GetById(notificationId);

            if (notification == null)
            {
                throw new Exception("Notification not found.");
            }

            // user can only delete their own notifications
            if (notification.RecipientId != recipientId && notification.RecipientId != 0)
            {
                throw new Exception("You can only delete your own notifications.");
            }

            await notificationRepository.DeleteById(notificationId);
        }

        // Method to get unread count for notification bell badge
        public async Task<int> GetUnreadCount(int recipientId)
        {
            int count = await notificationRepository.CountUnreadByRecipientId(recipientId);
            return count;
        }

        // Method to get all notifications (Admin panel)
        public async Task<List<NotificationResponseDTO>> GetAll()
        {
            List<NotificationModel> notifications = await notificationRepository.GetAll();

            List<NotificationResponseDTO> result = new List<NotificationResponseDTO>();

            foreach (NotificationModel notification in notifications)
            {
                result.Add(MapToDTO(notification));
            }
         
            return result;
        }

        // Method to handle comment added event from comment service
        public async Task HandleCommentAdded(CommentAddedDTO dto)
        {
            // NEW_COMMENT - someone commented on a post
            // notify the post author
            if (dto.NotificationType == "NEW_COMMENT")
            {
                
                await Send(
                    recipientId: dto.PostAuthorId,
                    actorId: dto.CommentAuthorId,
                    type: "NEW_COMMENT",
                    title: $"{dto.ActorName} commented on your post",
                    message: $"{dto.ActorName} left a comment on your post.",
                    relatedId: dto.PostId,
                    relatedType: "Post"
                );
            }

            // COMMENT_REPLY - someone replied to a comment
            // notify the original commenter
            if (dto.NotificationType == "COMMENT_REPLY")
            {
                await Send(
                    recipientId: dto.ParentCommentAuthorId,
                    actorId: dto.CommentAuthorId,
                    type: "COMMENT_REPLY",
                    title: $"{dto.ActorName} replied to your comment",
                    message: $"{dto.ActorName} replied to your comment.",
                    relatedId: dto.CommentId,
                    relatedType: "Comment"
                );
            }
        }

        // Method to handle like event from post service
        public async Task HandlePostLikedPersonalized(int postId, int postAuthorId, int actorId, string actorName)
        {
            // do not notify if user likes their own post
            if (postAuthorId == actorId)
            {
                return;
            }

            await Send(
                recipientId: postAuthorId,
                actorId: actorId,
                type: "LIKE",
                title: $"{actorName} liked your post",
                message: $"{actorName} liked your post.",
                relatedId: postId,
                relatedType: "Post"
            );
        }

        // Method to handle post published event from post service
        public async Task HandlePostPublished(int postId, string title, int authorId)
        {
            // 0 means global/system-wide
            await Send(
                recipientId: 0,
                actorId: authorId,
                type: "NEW_POST",
                title: "New post published!",
                message: $"A new post was published: {title}",
                relatedId: postId,
                relatedType: "Post"
            );
        }

        // Method to handle mention event from comment service
        public async Task HandleMention(int mentionedUserId, int actorId,
            int commentId, int postId)
        {
            // do not notify if user mentions themselves
            if (mentionedUserId == actorId)
            {
                return;
            }

            await Send(
                recipientId: mentionedUserId,
                actorId: actorId,
                type: "MENTION",
                title: "You were mentioned in a comment",
                message: "Someone mentioned you in a comment.",
                relatedId: commentId,
                relatedType: "Comment"
            );
        }

        // Method to get notifications by type
        public async Task<List<NotificationResponseDTO>> GetByType(string type)
        {
            List<NotificationModel> notifications = await notificationRepository.GetByType(type);
            List<NotificationResponseDTO> result = new List<NotificationResponseDTO>();

            foreach (NotificationModel notification in notifications)
            {
                result.Add(MapToDTO(notification));
            }
            return result;
        }

        // Method to get notifications related to a specific post or comment
        public async Task<List<NotificationResponseDTO>> GetByRelatedId(int relatedId)
        {
            List<NotificationModel> notifications = await notificationRepository.GetByRelatedId(relatedId);
            List<NotificationResponseDTO> result = new List<NotificationResponseDTO>();

            foreach (NotificationModel notification in notifications)
            {
                result.Add(MapToDTO(notification));
            }
            return result;
        }

        // Method to convert Notification model to DTO
        private NotificationResponseDTO MapToDTO(NotificationModel notification)
        {
            NotificationResponseDTO dto = new NotificationResponseDTO();
            dto.NotificationId = notification.NotificationId;
            dto.RecipientId = notification.RecipientId;
            dto.ActorId = notification.ActorId;
            dto.Type = notification.Type;
            dto.Title = notification.Title;
            dto.Message = notification.Message;
            dto.RelatedId = notification.RelatedId;
            dto.RelatedType = notification.RelatedType;
            dto.IsRead = notification.IsRead;
            dto.CreatedAt = notification.CreatedAt;
            return dto;
        }
    }
}