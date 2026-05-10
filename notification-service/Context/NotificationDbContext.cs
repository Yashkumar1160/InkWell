using InkWell.Notification.Models;
using Microsoft.EntityFrameworkCore;

namespace InkWell.Notification.Context
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        // Notifications table in database
        public DbSet<NotificationModel> Notifications { get; set; }
    }
}