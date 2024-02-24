namespace CatTok.Models
{
    public class Media
    {
        public int Id { get; set; }
        public Type MediaType { get; set; }
        public string MediaURL { get; set; }
        public string? Caption { get; set; }
        public string TimeStamp { get; set; }
    }

    public enum Type
    {
        IMAGE,
        VIDEO
    }
}