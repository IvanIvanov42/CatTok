using Cattok_API.Authentication;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cattok_API.Data.Models
{
    public class Media
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("media_type")]
        public string MediaType { get; set; } = string.Empty;

        [JsonPropertyName("media_url")]
        public string MediaUrl { get; set; } = string.Empty;

        [JsonPropertyName("caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        [JsonIgnore]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public InstagramUser User { get; set; }
    }
}
