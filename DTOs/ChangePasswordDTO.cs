namespace InkWell.Auth.DTOs
{
    // Data that Angular sends when user wants to change password
    public class ChangePasswordDTO
    {
        public string OldPassword{get;set;}
        public string NewPassword{get;set;}
    }
}