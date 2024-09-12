using Blazored.LocalStorage;
using CatTok.Models;
using CatTok.Services.IServices;
using System.Data;
using System.Net.Http.Json;

namespace CatTok.Services
{
    public class InstagramService : IInstagramService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public InstagramService(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
        {
            _httpClient = clientFactory.CreateClient("CatTokAPI");
            _localStorage = localStorage;

        }

        public async Task<IEnumerable<Media>?> GetMediasAsync()
        {
            var allMedias = await _httpClient.GetFromJsonAsync<List<Media>>("api/Instagram/GetInstagramData");
            return allMedias?.Where(media => media.MediaType == "IMAGE");
        }

        public async Task<IEnumerable<InstagramUser>> GetUsersWithMediaAsync()
        {
            var users = await _httpClient.GetFromJsonAsync<List<InstagramUser>>("api/Instagram/GetUsersMedia");
            if (users != null && users.Count > 0)
            {
                return users;
            }
            return new List<InstagramUser>();
        }


        public async Task<HttpResponseMessage> SendAuthorizationToken(string token)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Instagram/AuthorizeUser", token);
            return response;
        }

        public async Task<HttpResponseMessage> PostInstagramData()
        {
            var response = await _httpClient.PostAsync("api/Instagram/PostInstagramData", null);
            return response;
        }

        public async Task<bool> IsUserConnected()
        {
            var cachedStatus = await _localStorage.GetItemAsync<bool?>("isInstagramConnected");
            if (cachedStatus.HasValue)
            {
                return cachedStatus.Value;
            }

            var response = await _httpClient.GetAsync("api/Instagram/IsConnected");
            if (response.IsSuccessStatusCode)
            {
                var connectionStatus = await response.Content.ReadFromJsonAsync<InstagramConnectionStatus>();
                var isConnected = connectionStatus.IsConnected;
                await _localStorage.SetItemAsync("isInstagramConnected", isConnected);
                return isConnected;
            }

            return false;
        }
    }
}