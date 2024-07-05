using Cattok_API.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Cattok_API.Authentication
{
    public class InstagramUser : IdentityUser
    {
        public required ICollection<Media> Medias { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }

        public string? InstagramToken { get; set; }
        public DateTime InstagramTokenExpiry { get; set; }
        public string? InstagramUsername { get; set; }

        public bool IsInstagramTokenExpired()
        {
            return DateTime.UtcNow >= InstagramTokenExpiry;
        }
    }
}
