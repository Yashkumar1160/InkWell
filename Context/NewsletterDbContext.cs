using InkWell.Newsletter.Models;
using Microsoft.EntityFrameworkCore;

namespace InkWell.Newsletter.Context
{
    public class NewsletterDbContext : DbContext
    {
        public NewsletterDbContext(DbContextOptions<NewsletterDbContext> options) : base(options)
        {
        }

        // Subscribers table in database
        public DbSet<Subscriber> Subscribers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // no two subscribers can have the same email
            modelBuilder.Entity<Subscriber>()
                .HasIndex(s => s.Email)
                .IsUnique();

            // no two subscribers can have the same token
            modelBuilder.Entity<Subscriber>()
                .HasIndex(s => s.Token)
                .IsUnique();
        }
    }
}