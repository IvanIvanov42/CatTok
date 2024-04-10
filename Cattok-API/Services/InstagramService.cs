using Cattok_API.Data.Models;
using Cattok_API.Models;
using Cattok_API.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

public class InstagramService : IInstagramService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public InstagramService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<List<Media>?> GetInstagramDataAsync(string accessToken)
    {
        string apiUrl = $"https://graph.instagram.com/me/media?fields=id,media_type,media_url,caption,timestamp&access_token={accessToken}";

        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            var instagramResponse = await response.Content.ReadFromJsonAsync<InstagramResponseModel>();

            if (instagramResponse == null || instagramResponse.data == null)
                return null;

            return instagramResponse.data;
        }
        else
        {
            Console.WriteLine(response.StatusCode.ToString());
            return null;
        }
    }

    public async Task<string?> GetShortLivedAccessTokenAsync(string code)
    {
        var clientId = _configuration["ClientId"];
        var clientSecret = _configuration["ClientSecret"];
        var redirectUri = _configuration["RedirectUri"];

        var requestBody = new Dictionary<string, string>
        {
            {"client_id", clientId },
            {"client_secret", clientSecret},
            {"grant_type", "authorization_code"},
            {"redirect_uri", redirectUri},
            {"code", code}
        };

        var response = await _httpClient.PostAsync("https://api.instagram.com/oauth/access_token", new FormUrlEncodedContent(requestBody));

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

            return tokenResponse?.AccessToken;
        }

        return null;
    }

    public async Task<string?> GetLongLivedAccessTokenAsync(string shortLivedAccessToken)
    {
        var clientSecret = _configuration["ClientSecret"];
        string requestUrl = $"https://graph.instagram.com/access_token?grant_type=ig_exchange_token&client_secret={clientSecret}&access_token={shortLivedAccessToken}";

        var response = await _httpClient.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

            return tokenResponse?.AccessToken;
        }

        return null;
    }

    public async Task<string?> RefreshLongLivedAccessTokenAsync(string longLivedAccessToken)
    {
        string requestUrl = $"https://graph.instagram.com/refresh_access_token?grant_type=ig_refresh_token&access_token={longLivedAccessToken}";

        var response = await _httpClient.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

            return tokenResponse?.AccessToken;
        }

        return null;
    }
}