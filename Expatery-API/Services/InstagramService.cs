﻿using Expatery_API.Models;
using Expatery_API.Services;
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
