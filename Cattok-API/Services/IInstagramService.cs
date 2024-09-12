using Cattok_API.Data.Models;

namespace Cattok_API.Services
{
    public interface IInstagramService
    {
        Task<List<Media>?> GetInstagramDataAsync(string accessToken, string userId);
        Task<string?> GetInstagramUsername(string accessToken);
        Task<string?> GetShortLivedAccessTokenAsync(string code);
        Task<string?> GetLongLivedAccessTokenAsync(string shortLivedAccessToken);
        Task<string?> RefreshLongLivedAccessTokenAsync(string longLivedAccessToken);
    }
}
