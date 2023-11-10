using System;
using System.Net.Http;
using System.Threading.Tasks;

public class InstagramService
{
    private readonly HttpClient _httpClient;

    public InstagramService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string?> GetInstagramDataAsync(string accessToken)
    {
        string apiUrl = $"https://graph.instagram.com/me/media?fields=id&access_token={accessToken}";

        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            Console.WriteLine(response.StatusCode.ToString());
            return null;
        }
    }
}