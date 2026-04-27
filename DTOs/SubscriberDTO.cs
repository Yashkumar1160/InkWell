namespace InkWell.Newsletter.DTOs
{
    // data angular sends when user subscribes
    public class SubscribeDTO
    {
        public string Email { get; set; }
        public string FullName { get; set; }

        // set if subscriber is a registered user
        public int? UserId { get; set; }
    }
}