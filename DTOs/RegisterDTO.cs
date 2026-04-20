namespace InkWell.Auth.DTOs
{
    // Data that Angular sends when user wants to create account 
    public class RegisterDTO
    {
        public string Username{get;set;}
        public string Email{get;set;}
        public string Password { get; set; }
        public string FullName { get; set; }
    }
}