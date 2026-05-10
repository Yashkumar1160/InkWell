namespace InkWell.Auth.Models
{
    public class User
    {
        //Primary Key 
        public int UserId { get; set; }

        // Unique username
        public string Username { get; set; }

        // Unique email
        public string Email { get; set; }

        // Hashed Password
        public string PasswordHash{get;set;}

        // User's name 
        public string FullName { get; set;}

        // Roles (ADMIN, AUTHOR, READER)
        public string Role { get; set; }

        // Short Description on author profile
        public string Bio { get; set; }

        // Profile picture url
        public string AvatarUrl { get; set; }

        // How the user signed up
        public string Provider { get; set; }

        // If false the user is suspended and cannot login
        public bool IsActive{get;set;}

        // When account was created
        public DateTime CreatedAt { get; set; }


        // Constructor 
        // Sets default values when User() is called
        public User()
        {
            Username = "";
            Email = "";
            PasswordHash="";
            FullName="";
            Role="";
            Bio="";
            AvatarUrl="";
            Provider="";
            IsActive=true;
            CreatedAt=DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"Name: {FullName}";
        }
    }
}