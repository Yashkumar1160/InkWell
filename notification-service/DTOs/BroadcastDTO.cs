namespace InkWell.Notification.DTOs
{
    // data admin sends when broadcasting to all users
    public class BroadcastDTO
    {
        public string? Title { get; set; }
        public string Message { get; set; }

        // if set only users with this role get it
        // if empty sends to everyone
        public string? RoleFilter { get; set; }

        // list of recipient ids to send to
        // admin provides this list from user management
        public List<int> RecipientIds { get; set; }
    }
}