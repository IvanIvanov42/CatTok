using Cattok_API.Data.Models;

namespace Cattok_API.Data.Repository
{
    public interface IMediaRepository
    {
        Task<List<Media>> GetMediaListAsync(string userId);
        Task AddMediaAsync(string userId, List<Media> mediaList);
        Task DeleteMediaAsync(string userId);
    }
}
