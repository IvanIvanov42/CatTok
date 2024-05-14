using Azure.Security.KeyVault.Secrets;
using Cattok_API.Authentication;
using Cattok_API.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cattok_API.Data.Context
{
    public class MediaContext : IdentityDbContext
    {
        private readonly SecretClient _secretClient;

        public MediaContext(DbContextOptions<MediaContext> options, SecretClient secretClient)
            : base(options)
        {
            _secretClient = secretClient;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            KeyVaultSecret secretSqlConnection = _secretClient.GetSecret("SQL-CATTOK");
            string azureSqlConnection = secretSqlConnection.Value;
            optionsBuilder.UseSqlServer(azureSqlConnection);
        }

        public DbSet<Media> Medias { get; set; }
        public DbSet<InstagramUser> InstagramUsers { get; set; }
    }
}
