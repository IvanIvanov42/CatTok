using Expatery_API.Models;

namespace Expatery_API.Services
{
    public interface IInstagramDataStorage
    {
        Task<string> GetLatestTimestamp();
        Task UpdateLatestTimestamp(string newTimestamp);
        Task<List<Media>> GetMediaListAsync();
        Task AddMediaAsync(List<Media> mediaList);
    }
}