using CatTok.Models;
using CatTok.Services.IServices;
using System.Net.Http.Json;

namespace CatTok.Services
{
    public class InstagramService : IInstagramService
    {
        private readonly HttpClient _httpClient;
        
        public InstagramService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("CatTokAPI"); ;
        }

        public async Task<IEnumerable<Media>?> GetMediasAsync()
        {
            var allMedias = await _httpClient.GetFromJsonAsync<List<Media>>("api/Instagram/GetInstagramData");
            return allMedias?.Where(media => media.MediaType == "IMAGE");
        }


        public string GetUser()
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> SendAuthorizationToken(string token)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Instagram/AuthorizeUser", token);
            return response;
        }
    }
}