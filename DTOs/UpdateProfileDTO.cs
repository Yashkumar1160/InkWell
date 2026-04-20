namespace InkWell.Auth.DTOs
{
    // Data that Angular sends when user wants to update their profile
    public class UpdateProfileDTO
    {
        public string FullName { get; set; }
        public string Bio {get;set;}
        public string  AvatarUrl { get; set; }
    }
}