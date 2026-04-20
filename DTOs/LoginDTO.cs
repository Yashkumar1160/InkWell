namespace InkWell.Auth.DTOs
{
    // Data that Angular sends when user wants to login 

    public class LoginDTO
    {
        public string Email{get;set;}
        public string Password{get;set;}
    }
}