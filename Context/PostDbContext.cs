using InkWell.Post.Models;
using Microsoft.EntityFrameworkCore;


namespace InkWell.Post.Context
{
    public class PostDbContext : DbContext
    {
        // Constructor
        public PostDbContext(DbContextOptions<PostDbContext> options) : base(options)
        {
        }

        // Posts table in database
        public DbSet<PostModel> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // slug is used in the URL so it must be unique
            modelBuilder.Entity<PostModel>()
                .HasIndex(p => p.Slug)
                .IsUnique();
        }
    }
}