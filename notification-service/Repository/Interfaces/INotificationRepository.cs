using InkWell.Notification.Models;

namespace InkWell.Notification.Repository.Interfaces
{
    public interface INotificationRepository
    {
        // Method to get notification by id
        Task<NotificationModel> GetById(int id);

        // Method to get all notifications for a specific user
        Task<List<NotificationModel>> GetByRecipientId(int recipientId);

        // Method to get only unread notifications for a user
        Task<List<NotificationModel>> GetUnreadByRecipientId(int recipientId);

        // Method to get notifications filtered by type
        Task<List<NotificationModel>> GetByType(string type);

        // Method to get notifications related to a specific post or comment
        Task<List<NotificationModel>> GetByRelatedId(int relatedId);

        // Method to count unread notifications for a user (used for bell badge number)
        Task<int> CountUnreadByRecipientId(int recipientId);

        // Method to get all notifications (Admin use)
        Task<List<NotificationModel>> GetAll();

        // Method to save new notification
        Task<NotificationModel> Add(NotificationModel notification);

        // Method to save changes to existing notification
        Task<NotificationModel> Update(NotificationModel notification);

        // Method to delete single notification by id
        Task DeleteById(int id);

        // Method to delete all read notifications for a user
        Task DeleteReadByRecipientId(int recipientId);


        // Method to delete notifications for a user filtered by read status
        Task DeleteByRecipientIdAndIsRead(int recipientId, bool isRead);
    }
}