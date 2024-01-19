// InstagramDataStorageDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace Expatery_API.Models
{
    public class InstagramDataStorageDbContext : DbContext
    {
        public DbSet<Media> Medias { get; set; }

        public InstagramDataStorageDbContext(DbContextOptions<InstagramDataStorageDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optionally configure additional settings here
            // For example, specifying table name:
            modelBuilder.Entity<Media>().ToTable("Media");

            base.OnModelCreating(modelBuilder);
        }
    }
}