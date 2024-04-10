using Cattok_API.Authentication;
using Cattok_API.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cattok_API.Data.Context
{
    public class MediaContext : IdentityDbContext
    {
        public MediaContext(DbContextOptions<MediaContext> options)
            : base(options)
        {
        }
        public DbSet<Media> Medias { get; set; }
        public DbSet<InstagramUser> InstagramUsers { get; set; }
    }
}
