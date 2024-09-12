using CatTok.Models;

namespace CatTok.Services.IServices
{
    public interface IInstagramService
    {
        Task<HttpResponseMessage> SendAuthorizationToken(string token);
        Task<IEnumerable<InstagramUser>> GetUsersWithMediaAsync();
        Task<IEnumerable<Media>?> GetMediasAsync();
        Task<HttpResponseMessage> PostInstagramData();
        Task<bool> IsUserConnected();
    }
}