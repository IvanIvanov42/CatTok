namespace Expatery_API.Services
{
    public interface IInstagramDataStorage
    {
        string GetLatestTimestamp();
        void UpdateLatestTimestamp(string newTimestamp);
    }
}