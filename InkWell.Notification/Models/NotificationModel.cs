using System.ComponentModel.DataAnnotations;

namespace InkWell.Notification.Models
{
    public class NotificationModel
    {
        // primary key
        [Key]
        public int NotificationId { get; set; }

        // who receives this notification
        public int RecipientId { get; set; }

        // who triggered this notification
        public int ActorId { get; set; }

        // notification type(NEW_COMMENT, COMMENT_REPLY, MENTION, NEW_POST, LIKE)
        public string Type { get; set; }

        // short title shown in notification bell
        public string Title { get; set; }

        // full message shown when notification is opened
        public string Message { get; set; }

        // id of the related item (post or comment)
        public int RelatedId { get; set; }

        // Related Type(Post or Comment)
        public string RelatedType { get; set; }

        // false = unread
        // true  = read
        public bool IsRead { get; set; }

        // when this notification was created
        public DateTime CreatedAt { get; set; }

        // Constructor to set default values
        public NotificationModel()
        {
            Type = "";
            Title = "";
            Message = "";
            RelatedType = "";
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }
    }
}