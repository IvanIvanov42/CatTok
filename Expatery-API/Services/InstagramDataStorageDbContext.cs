// InstagramDataStorageDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace Expatery_API.Models
{
    public class InstagramDataStorageDbContext : DbContext
    {
        public DbSet<InstagramTimeStamp> InstagramTimeStamps { get; set; }

        public InstagramDataStorageDbContext(DbContextOptions<InstagramDataStorageDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optionally configure additional settings here
            // For example, specifying table name:
            modelBuilder.Entity<InstagramTimeStamp>().ToTable("InstagramTimeStamp");

            base.OnModelCreating(modelBuilder);
        }
    }
}
