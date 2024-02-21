using CatTok.Services.IServices;
using static System.Net.WebRequestMethods;

namespace CatTok.Services
{
    public class InstagramService : IInstagramService
    {
        private readonly HttpClient _httpClient;

        public InstagramService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("CatTokAPI"); ;
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