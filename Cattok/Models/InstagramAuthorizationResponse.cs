namespace CatTok.Models
{
    public class InstagramAuthorizationResponse
    {
        public required string AccessToken { get; set; }
        public required string InstagramUsername { get; set; }
    }
}
