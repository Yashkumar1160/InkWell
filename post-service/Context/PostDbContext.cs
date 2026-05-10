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
        
        // Likes table to track who liked what
        public DbSet<LikeModel> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // slug is used in the URL so it must be unique
            modelBuilder.Entity<PostModel>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            // A user can only like a post once
            modelBuilder.Entity<LikeModel>()
                .HasIndex(l => new { l.PostId, l.UserId })
                .IsUnique();
        }
    }
}