namespace InkWell.Shared.Events
{
    public class NewsletterPublishedEvent
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentAt { get; set; }
    }
}
