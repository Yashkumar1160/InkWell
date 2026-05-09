using InkWell.Category.Models;
using Microsoft.EntityFrameworkCore;

namespace InkWell.Category.Context
{
    public class CategoryDbContext : DbContext
    {
        public CategoryDbContext(DbContextOptions<CategoryDbContext> options) : base(options)
        {
        }

        // Categories table
        public DbSet<CategoryModel> Categories { get; set; }

        // Tags table
        public DbSet<Tag> Tags { get; set; }

        // PostTags table
        public DbSet<PostTag> PostTags { get; set; }

        // PostCategories table
        public DbSet<PostCategory> PostCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryModel>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Slug)
                .IsUnique();

            modelBuilder.Entity<PostTag>()
                .HasIndex(pt => new { pt.PostId, pt.TagId })
                .IsUnique();

            // a post can only be in each category once
            modelBuilder.Entity<PostCategory>()
                .HasIndex(pc => new { pc.PostId, pc.CategoryId })
                .IsUnique();
        }
    }
}