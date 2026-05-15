namespace InkWell.Auth.DTOs
{
    // What we send back to Angular after register/login
    public class AuthResponseDTO
    {
        public int UserId{get;set;}
        public string Username{get;set;}
        public string Email{get;set;}
        public string FullName{get;set;}
        public string Role{get;set;}
        public string Token{get;set;}
    }
}