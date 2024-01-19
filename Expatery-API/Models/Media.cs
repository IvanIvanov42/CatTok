using System.ComponentModel.DataAnnotations.Schema;

namespace Expatery_API.Models
{
    public class Media
    {
        public string id { get; set; }
        public string media_type { get; set; }
        public string media_url { get; set; }
        public string? caption { get; set; }

        [Column("time_stamp")]
        public string timestamp { get; set; }
    }
}