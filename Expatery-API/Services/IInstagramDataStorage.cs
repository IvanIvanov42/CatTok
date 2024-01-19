using Expatery_API.Models;

namespace Expatery_API.Services
{
    public interface IInstagramDataStorage
    {
        Task<List<Media>> GetMediaListAsync();

        Task AddMediaAsync(List<Media> mediaList);
    }
}