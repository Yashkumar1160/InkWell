namespace InkWell.Notification.DTOs
{
    // what we send back to angular
    public class NotificationResponseDTO
    {
        public int NotificationId { get; set; }
        public int RecipientId { get; set; }
        public int ActorId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int RelatedId { get; set; }
        public string RelatedType { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}