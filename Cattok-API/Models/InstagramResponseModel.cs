using Cattok_API.Data.Models;
using System.Text.Json.Serialization;

namespace Cattok_API.Models
{
    public class InstagramMediaResponse
    {
        public List<Media> data { get; set; }
        public Paging paging { get; set; }
    }

    public class InstagramProfileResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }
    }

    public class Paging
    {
        public Cursors cursors { get; set; }
    }

    public class Cursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }
}