namespace InkWell.Newsletter.DTOs
{

    // what we send back to angular
    public class SubscriberResponseDTO
    {
        public int SubscriberId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; }
        public DateTime SubscribedAt { get; set; }
        public DateTime? UnsubscribedAt { get; set; }
        public string Preferences { get; set; }
    }
}