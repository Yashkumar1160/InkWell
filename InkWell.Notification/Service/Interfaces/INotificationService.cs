using System.Collections.Generic;
using System.Threading.Tasks;
using InkWell.Notification.DTOs;

namespace InkWell.Notification.Services.Interfaces
{
    public interface INotificationService
    {
        // Method to create and save a single notification
        Task<NotificationResponseDTO> Send(int recipientId, int actorId,
            string type, string title, string message,
            int relatedId, string relatedType);

        // Method to send notification to multiple recipients at once (Admin broadcast)
        Task SendBulk(BroadcastDTO dto);

        // Method to get all notifications for a user
        Task<List<NotificationResponseDTO>> GetByRecipient(int recipientId);

        // Method to get unread notifications for a user
        Task<List<NotificationResponseDTO>> GetUnread(int recipientId);

        // Method to mark a single notification as read
        Task MarkAsRead(int notificationId, int recipientId);

        // Method to mark all notifications as read for a user
        Task MarkAllRead(int recipientId);

        // Method to delete all read notifications for a user
        Task DeleteRead(int recipientId);

        // Method to delete a single notification
        Task DeleteNotification(int notificationId, int recipientId);

        // Method to get unread count for notification bell badge
        Task<int> GetUnreadCount(int recipientId);

        // Method to get all notifications (Admin panel)
        Task<List<NotificationResponseDTO>> GetAll();

        // Method to handle comment added event from comment service
        Task HandleCommentAdded(CommentAddedDTO dto);

        // Method to handle like event from post service
        Task HandlePostLikedPersonalized(int postId, int postAuthorId, int actorId, string actorName);

        // Method to handle mention event from comment service
        Task HandleMention(int mentionedUserId, int actorId, int commentId, int postId);

        // Method to handle post published event from post service
        Task HandlePostPublished(int postId, string title, int authorId);

        // Method to get notifications by type
        Task<List<NotificationResponseDTO>> GetByType(string type);

        // Method to get notifications related to a specific post or comment
        Task<List<NotificationResponseDTO>> GetByRelatedId(int relatedId);
    }
}