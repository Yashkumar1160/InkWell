using InkWell.Comment.Models;
using Microsoft.EntityFrameworkCore;

namespace InkWell.Comment.Context
{
    public class CommentDbContext : DbContext
    {
        public CommentDbContext(DbContextOptions<CommentDbContext> options) : base(options)
        {
        }

        // Comments table in database
        public DbSet<CommentModel> Comments { get; set; }
    }
}