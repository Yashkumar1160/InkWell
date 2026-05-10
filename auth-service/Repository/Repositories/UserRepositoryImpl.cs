using InkWell.Auth.Context;
using InkWell.Auth.Models;
using InkWell.Auth.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace InkWell.Auth.Repository.Repositories
{
    public class UserRepositoryImpl : IUserRepository
    {
        // dbContext is connection to the database
        private AuthDbContext dbContext;

        // Constructor Dependency Injection
        public UserRepositoryImpl(AuthDbContext context)
        {
            dbContext = context;
        }

        // Find user by email
        public async Task<User> GetByEmail(string email)
        {
            User user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        // Find user by id 
        public async Task<User> GetById(int id)
        {
            User user = await dbContext.Users.FindAsync(id);
            return user;
        }

        // Get user by username
        public async Task<User> GetByUsername(string username)
        {
            User user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }

        // Add new user to database
        public async Task<User> Add(User user)
        {
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return user;
        }


        // Update User 
        public async Task<User> Update(User user)
        {
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        // Check if email is already registered
        public async Task<bool> EmailExists(string email)
        {
            bool exists = await dbContext.Users.AnyAsync(u => u.Email == email);
            return exists;

        }

        // Check if username is already taken
        public async Task<bool> UsernameExists(string username)
        {
            bool exists = await dbContext.Users.AnyAsync(u => u.Username == username);
            return exists;
        }

        // Get users by a specific 
        public async Task<List<User>> GetAllByRole(string role)
        {
            List<User> users = await dbContext.Users.Where(u => u.Role == role).ToListAsync();
            return users;
        }

        // Get all users
        public async Task<List<User>> GetAll()
        {
            List<User> users = await dbContext.Users.ToListAsync();
            return users;
        }

        // Search user where username contains the keyword
        public async Task<List<User>> Search(string keyword)
        {
            List<User> users = await dbContext.Users.Where(u => u.Username.Contains(keyword)).ToListAsync();
            return users;
        }

        // Delete a user from database
        public async Task DeleteById(int id)
        {
            // Find user from database
            User user = await dbContext.Users.FindAsync(id);

            if (user != null)
            {
                // Remove user from database
                dbContext.Users.Remove(user);
                await dbContext.SaveChangesAsync();
            }
        }

    }
}