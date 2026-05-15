using InkWell.Auth.DTOs;

namespace InkWell.Auth.Services.Interfaces
{
    public interface IAuthService
    {
        // Create new account (returns token so user is logged immediately)
        Task<AuthResponseDTO> Register(RegisterDTO dto);

        // Check credentials and return token if correct
        Task<AuthResponseDTO> Login(LoginDTO dto);

        // Get profile info for logged in user 
        Task<object> GetProfile(int userId);

        // Get user by email
        Task<object> GetUserByEmail(string email);

        // Get all users
        Task<List<object>> GetAllUsers();

        // Get users by specific role
        Task<List<object>> GetUsersByRole(string role);

        // Update users bio , avatar , fullname
        Task<object> UpdateProfile(int userId, UpdateProfileDTO dto);

        // Change password after verifying old one
        Task ChangePassword(int userId, ChangePasswordDTO dto);

        // Disable account so user cannot login (ADMIN action)
        Task Deactivate(int userId);

        // Change user role to (READER, AUTHOR or ADMIN) ADMIN action
        Task ChangeRole(int userId, string role);

        // Seach users with specific keyword
        Task<List<object>> SearchUsers(string keyword);

        // Logout User
        Task Logout();

        // Validate JWT Token
        bool ValidateToken(string token);

        // Refresh Token
        Task<AuthResponseDTO> RefreshToken(string token);

        // Reactivate suspended account (admin only)
        Task Reactivate(int userId);

        // Delete account from database (admin only)
        Task DeleteUser(int userId);
        Task<AuthResponseDTO> GoogleLogin(string idToken);
    }
}