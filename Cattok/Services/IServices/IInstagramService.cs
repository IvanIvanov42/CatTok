using CatTok.Models;

namespace CatTok.Services.IServices
{
    public interface IInstagramService
    {
        Task<InstagramAuthorizationResponse?> SendAuthorizationToken(string token);
        Task<HttpResponseMessage> UnauthorizeUser();
        Task<IEnumerable<InstagramUser>> GetUsersWithMediaAsync();
        Task<IEnumerable<Media>?> GetMediasAsync();
        Task<HttpResponseMessage> PostInstagramData();
        Task<bool> IsUserConnected();
    }
}