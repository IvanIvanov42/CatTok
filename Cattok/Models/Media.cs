using System.Text.Json.Serialization;

namespace CatTok.Models
{
    public class Media
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("mediaType")]
        public required string MediaType { get; set; }
        [JsonPropertyName("mediaUrl")]
        public required string MediaUrl { get; set; }
        [JsonPropertyName("caption")]
        public string? Caption { get; set; }
        [JsonPropertyName("timestamp")]
        public required string Timestamp { get; set; }
    }
}