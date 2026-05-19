using System.Collections.Generic;

namespace InkWell.Shared.Events
{
    public class SendInAppPostNotificationEvent
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public List<int> RecipientIds { get; set; }
    }
}
