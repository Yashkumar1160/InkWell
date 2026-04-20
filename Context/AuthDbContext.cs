using InkWell.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace InkWell.Auth.Context
{
    public class AuthDbContext:DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext>options):base(options){ }
        

        // Users table in database
        public DbSet<User>Users{get;set;}


        // Runs when EF core creates/updates DB (used to add extra rules that cannot be set via properties)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // No two users can have same email
            modelBuilder.Entity<User>().HasIndex(u=>u.Email).IsUnique();

            // No two users can have same username
            modelBuilder.Entity<User>().HasIndex(u=>u.Username).IsUnique();
        }
    }
}