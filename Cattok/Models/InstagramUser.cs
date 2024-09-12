namespace CatTok.Models
{
    public class InstagramUser
    {
        public string Id { get; set; }
        public string? InstagramUsername { get; set; }
        public List<Media> Medias { get; set; } = new List<Media>();
    }

    public class InstagramConnectionStatus
    {
        public bool IsConnected { get; set; }
    }
}
