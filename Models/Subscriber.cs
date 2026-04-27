using System.ComponentModel.DataAnnotations;

namespace InkWell.Newsletter.Models
{
    public class Subscriber
    {
        // primary key
        [Key]
        public int SubscriberId { get; set; }

        // email address of the subscriber
        public string Email { get; set; }

        // UserId from auth service
        public int? UserId { get; set; }

        // Subscriber name
        public string FullName { get; set; }

        // Subscription Status (PENDING, ACTIVE, UNSUBSCRIBED)
        public string Status { get; set; }

        // when the subscription was created
        public DateTime SubscribedAt { get; set; }

        // when the user unsubscribed (null if active or pending)
        public DateTime? UnsubscribedAt { get; set; }

        // GUID token used for two things
        // 1. confirmation link in welcome email
        // 2. one click unsubscribe link in every email
        public string Token { get; set; }

        // comma separated preference tags
        public string Preferences { get; set; }

        // when the current token was generated
        public DateTime TokenCreatedAt { get; set; }

        // Constructor to set default values
        public Subscriber()
        {
            Email = "";
            FullName = "";
            Status = "PENDING";
            SubscribedAt = DateTime.UtcNow;
            UnsubscribedAt = null;
            Token = Guid.NewGuid().ToString();
            TokenCreatedAt = DateTime.UtcNow;
            Preferences = "";
        }
    }
}