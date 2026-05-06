using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InkWell.Notification.Context;
using InkWell.Notification.Models;
using InkWell.Notification.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InkWell.Notification.Repository.Repositories
{
    public class NotificationRepositoryImpl : INotificationRepository
    {
        // NotificationDbContext  instance
        private NotificationDbContext dbContext;

        // Constructor Dependency Injection
        public NotificationRepositoryImpl(NotificationDbContext context)
        {
            dbContext = context;
        }

        // Method to get notification by id
        public async Task<NotificationModel> GetById(int id)
        {
            NotificationModel notification = await dbContext.Notifications.FindAsync(id);
            return notification;
        }

        // Method to get all notifications for a user ordered newest first
        public async Task<List<NotificationModel>> GetByRecipientId(int recipientId)
        {
            List<NotificationModel> notifications = await dbContext.Notifications
                .Where(n => n.RecipientId == recipientId || n.RecipientId == 0)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return notifications;
        }

        // Method to get only unread notifications for a user
        public async Task<List<NotificationModel>> GetUnreadByRecipientId(int recipientId)
        {
            List<NotificationModel> notifications = await dbContext.Notifications
                .Where(n => (n.RecipientId == recipientId || n.RecipientId == 0) && n.IsRead == false)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return notifications;
        }

        // Method to get notifications by type
        public async Task<List<NotificationModel>> GetByType(string type)
        {
            List<NotificationModel> notifications = await dbContext.Notifications
                .Where(n => n.Type == type)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return notifications;
        }

        // Method to get notifications for a specific post or comment
        public async Task<List<NotificationModel>> GetByRelatedId(int relatedId)
        {
            List<NotificationModel> notifications = await dbContext.Notifications
                .Where(n => n.RelatedId == relatedId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return notifications;
        }

        // Method to count unread notifications (used for bell badge number)
        public async Task<int> CountUnreadByRecipientId(int recipientId)
        {
            int count = await dbContext.Notifications
                .CountAsync(n => (n.RecipientId == recipientId || n.RecipientId == 0) && n.IsRead == false);
            return count;
        }

        // Method to get all notifications (Admin use)
        public async Task<List<NotificationModel>> GetAll()
        {
            List<NotificationModel> notifications = await dbContext.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return notifications;
        }

        // Method to save new notification
        public async Task<NotificationModel> Add(NotificationModel notification)
        {
            dbContext.Notifications.Add(notification);
            await dbContext.SaveChangesAsync();
            return notification;
        }

        // Method to save changes to notification
        public async Task<NotificationModel> Update(NotificationModel notification)
        {
            dbContext.Notifications.Update(notification);
            await dbContext.SaveChangesAsync();
            return notification;
        }

        // Method to delete single notification by id
        public async Task DeleteById(int id)
        {
            NotificationModel notification = await dbContext.Notifications.FindAsync(id);
            if (notification != null)
            {
                dbContext.Notifications.Remove(notification);
                await dbContext.SaveChangesAsync();
            }
        }

        // Method to delete all read notifications for a user
        // user clears their notification history
        public async Task DeleteReadByRecipientId(int recipientId)
        {
            List<NotificationModel> readNotifications = await dbContext.Notifications
                .Where(n => n.RecipientId == recipientId && n.IsRead == true)
                .ToListAsync();

            dbContext.Notifications.RemoveRange(readNotifications);
            await dbContext.SaveChangesAsync();
        }

        // Method to delete notifications by recipient id and read status
        public async Task DeleteByRecipientIdAndIsRead(int recipientId, bool isRead)
        {
            List<NotificationModel> notifications = await dbContext.Notifications
                .Where(n => n.RecipientId == recipientId && n.IsRead == isRead)
                .ToListAsync();

            dbContext.Notifications.RemoveRange(notifications);
            await dbContext.SaveChangesAsync();
        }
    }
}