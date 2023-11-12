namespace Expatery_API.Models
{
    public class InstagramResponseModel
    {
        public List<Data> data { get; set; }
        public Paging paging { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
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

    public class Media
    {
        public string id { get; set; }
        public string media_type { get; set; }
        public string media_url { get; set; }
        public string username { get; set; }
        public string timestamp { get; set; }
    }
}