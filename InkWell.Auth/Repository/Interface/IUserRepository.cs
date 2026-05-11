using InkWell.Auth.Models;

namespace InkWell.Auth.Repository.Interface
{
    public interface IUserRepository
    {
        // Find user by email
        Task<User> GetByEmail(string email);

        // Find user by user id 
        Task<User> GetById(int id);

        // Find user by username
        Task<User> GetByUsername(string username);

        // Save a new user to database 
        Task<User> Add(User user);

        // Update user
        Task<User> Update(User user);

        // Check if email is already registered
        Task<bool> EmailExists(string email);

        // Check if username is already taken
        Task<bool> UsernameExists(string username);

        // Get all users by specific role
        Task<List<User>> GetAllByRole(string role);

        // Get all users
        Task<List<User>> GetAll();

        // Search users by username keyword
        Task<List<User>> Search(string keyword);

        // Delete user from database
        Task DeleteById(int id);

    }
}