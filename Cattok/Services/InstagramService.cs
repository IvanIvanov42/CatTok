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
            var allMedias = await _httpClient.GetFromJsonAsync<IEnumerable<Media>>("api/Instagram/GetInstagramData");
            return allMedias.Where(media => media.MediaType == "IMAGE");
        }

        public async Task<SecretsResponse> GetSecrets()
        {
            var response = await _httpClient.GetFromJsonAsync<SecretsResponse>("api/Instagram/GetSecrets");
            return response;
        }

        public string GetUser()
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> SendAuthorizationToken(string token)
        {
            return await _httpClient.PostAsJsonAsync("Instagram/AuthorizeUser", token);
        }
    }
}