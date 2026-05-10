using InkWell.Media.Models;
using Microsoft.EntityFrameworkCore;

namespace InkWell.Media.Context
{
    public class MediaDbContext : DbContext
    {
        public MediaDbContext(DbContextOptions<MediaDbContext> options) : base(options)
        {
        }

        // MediaFiles table in database
        public DbSet<MediaModel> MediaFiles { get; set; }
    }
}