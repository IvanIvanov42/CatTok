using Cattok_API.Models;

namespace Cattok_API.Services
{
    public interface IInstagramDataStorage
    {
        Task<string> GetLatestTimestamp();
        Task UpdateLatestTimestamp(string newTimestamp);
        Task<List<Media>> GetMediaListAsync();
        Task AddMediaAsync(List<Media> mediaList);
    }
}