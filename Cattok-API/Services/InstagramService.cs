using Cattok_API.Data.Models;
using Cattok_API.Models;
using Cattok_API.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;

public class InstagramService : IInstagramService
{
    private readonly HttpClient _httpClient;
    private readonly SecretClient _secretClient;

    public InstagramService(HttpClient httpClient, SecretClient secretClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _secretClient = secretClient ?? throw new ArgumentNullException(nameof(secretClient));
    }

    public async Task<List<Media>?> GetInstagramDataAsync(string accessToken, string userId)
    {
        string apiUrl = $"https://graph.instagram.com/me/media?fields=id,media_type,media_url,caption,timestamp&access_token={accessToken}";

        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            var instagramResponse = await response.Content.ReadFromJsonAsync<InstagramMediaResponse>();

            if (instagramResponse == null || instagramResponse.data == null)
                return null;

            foreach (var media in instagramResponse.data)
            {
                media.UserId = userId;
            }


            return instagramResponse.data;
        }
        else
        {
            Console.WriteLine(response.StatusCode.ToString());
            return null;
        }
    }

    public async Task<string?> GetInstagramUsername(string accessToken)
    {
        string apiUrl = $"https://graph.instagram.com/me?fields=id,username&access_token={accessToken}";

        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            var instagramResponse = await response.Content.ReadFromJsonAsync<InstagramProfileResponse>();

            if (instagramResponse == null || instagramResponse.Username == null)
                return null;

            return instagramResponse.Username;
        }
        else
        {
            Console.WriteLine(response.StatusCode.ToString());
            return null;
        }
    }

    public async Task<string?> GetShortLivedAccessTokenAsync(string code)
    {
        var clientId = await GetSecretAsync("ClientId");
        var clientSecret = await GetSecretAsync("ClientSecret");
        var redirectUri = await GetSecretAsync("RedirectUri");

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
        var clientSecret = await GetSecretAsync("ClientSecret");
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

    private async Task<string> GetSecretAsync(string secretName)
    {
        KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);
        return secret.Value;
    }
}