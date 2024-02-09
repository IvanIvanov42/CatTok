using Cattok_API.Models;
using Cattok_API.Services;
using System.Text.Json;

public class InstagramService
{
    private readonly HttpClient _httpClient;
    private readonly IInstagramDataStorage _instagramDataStorage;

    public InstagramService(HttpClient httpClient, IInstagramDataStorage instagramDataStorage)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _instagramDataStorage = instagramDataStorage ?? throw new ArgumentNullException(nameof(instagramDataStorage));
    }

    public async Task<string?> GetInstagramDataAsync(string accessToken, string latestTimestamp)
    {
        string apiUrl = $"https://graph.instagram.com/me/media?fields=id,timestamp&since={latestTimestamp[..^4]}&access_token={accessToken}";

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

    public async Task<string?> GetAuthorizationAsync(string clientId, string redirectId)
    {
        string apiUrl = $"https://api.instagram.com/oauth/authorize?client_id={clientId}&redirect_uri={redirectId}&scope=user_profile,user_media&response_type=code";

        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

        return await response.Content.ReadAsStringAsync();
    }

    //public async Task<string?> Get

    public async Task<List<Media>?> GetMediaDetailsAsync(List<string> listOfIds, string accessToken)
    {
        List<Media> mediaList = new List<Media>();
        foreach (string id in listOfIds)
        {
            string apiUrl = $"https://graph.instagram.com/{id}?fields=id,media_type,media_url,caption,timestamp&access_token={accessToken}";
            HttpResponseMessage mediaResponse = await _httpClient.GetAsync(apiUrl);
            if (mediaResponse.IsSuccessStatusCode)
            {
                string mediaData = await mediaResponse.Content.ReadAsStringAsync();
                Media media = JsonSerializer.Deserialize<Media>(mediaData);
                mediaList.Add(media);
            }
            else
            {
                Console.WriteLine($"Failed to fetch media details for ID: {id}. Status Code: {mediaResponse.StatusCode}");
            }
        }
        return mediaList;
    }

    public async Task<List<Media>?> GetMediaFromDb()
    {
        try
        {
            // Call the method in your storage service to get media data from the database
            List<Media> mediaList = await _instagramDataStorage.GetMediaListAsync();

            return mediaList;
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine(ex);
            throw; // Re-throw the exception to be handled by the controller
        }
    }

    public async Task UpdateLatestTimeStampAsync(InstagramDataStorageDbContext dbContext, string newTimestamp)
    {
        var timeStamp = new InstagramTimeStamp { LatestTimeStamp = newTimestamp };
        dbContext.InstagramTimeStamps.Add(timeStamp);
        await dbContext.SaveChangesAsync();
    }
}