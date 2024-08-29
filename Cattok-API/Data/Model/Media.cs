using Cattok_API.Authentication;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cattok_API.Data.Models
{
    public class Media
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("media_type")]
        public required string MediaType { get; set; }
        [JsonPropertyName("media_url")]
        public required string MediaUrl { get; set; }
        [JsonPropertyName("caption")]
        public string? Caption { get; set; }
        [JsonPropertyName("timestamp")]
        public required string Timestamp { get; set; }

        public required string UserId { get; set; }

        [ForeignKey("UserId")]
        public InstagramUser User { get; set; }
    }
}