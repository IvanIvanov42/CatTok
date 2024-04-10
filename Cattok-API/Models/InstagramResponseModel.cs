using Cattok_API.Data.Models;

namespace Cattok_API.Models
{
    public class InstagramResponseModel
    {
        public List<Media> data { get; set; }
        public Paging paging { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public string media_type { get; set; }
        public string media_url { get; set; }
        public string? caption { get; set; }
        public string timestamp { get; set; }
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