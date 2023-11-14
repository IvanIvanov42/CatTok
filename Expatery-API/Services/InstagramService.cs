using Expatery_API.Models;
using System.Text.Json;

public class InstagramService
{
    private readonly HttpClient _httpClient;

    public InstagramService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string?> GetInstagramDataAsync(string accessToken, string latestTimestamp)
    {
        string apiUrl = $"https://graph.instagram.com/me/media?fields=id,timestamp&since={latestTimestamp}&access_token={accessToken}";

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

    public async Task<List<Media>?> GetMediaDetailsAsync(List<string> listOfIds, string accessToken)
    {
        List<Media> mediaList = new List<Media>();
        foreach (string id in listOfIds)
        {
            string mediaApiUrl = $"https://graph.instagram.com/{id}?fields=id,media_type,media_url,username,timestamp&access_token={accessToken}";
            HttpResponseMessage mediaResponse = await _httpClient.GetAsync(mediaApiUrl);
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

    public async Task UpdateLatestTimeStampAsync(InstagramDataStorageDbContext dbContext, string newTimestamp)
    {
        var timeStamp = new InstagramTimeStamp { LatestTimeStamp = newTimestamp };
        dbContext.InstagramTimeStamps.Add(timeStamp);
        await dbContext.SaveChangesAsync();
    }
}
