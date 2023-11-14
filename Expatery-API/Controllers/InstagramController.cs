﻿using Expatery_API.Models;
using Expatery_API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Expatery_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstagramController : ControllerBase
    {
        private readonly InstagramService _instagramService;
        private readonly IInstagramDataStorage _instagramDataStorage;

        public InstagramController(InstagramService instagramService, IInstagramDataStorage instagramDataStorage)
        {
            _instagramService = instagramService;
            _instagramDataStorage = instagramDataStorage;
        }

        [HttpGet("GetInstagramData")]
        public async Task<IActionResult> GetInstagramData()
        {
            string? accessToken = HttpContext.Items["InstagramAccessToken"] as string;

            if (string.IsNullOrEmpty(accessToken))
            {
                // Handle the case where the access token is not available
                return StatusCode(500, "Instagram access token not found.");
            }

            try
            {
                // Retrieve the latest stored timestamp or ID
                string latestTimestamp = _instagramDataStorage.GetLatestTimestamp();

                // Use the access token and latest timestamp in your InstagramService
                var instagramData = await _instagramService.GetInstagramDataAsync(accessToken, latestTimestamp);

                InstagramResponseModel? responseModel = JsonSerializer.Deserialize<InstagramResponseModel>(instagramData);

                if (responseModel != null)
                {
                    List<string> listOfIds = responseModel.data.Select(item => item.id).ToList();

                    // Call the new method to get media details
                    List<Media> mediaList = await _instagramService.GetMediaDetailsAsync(listOfIds, accessToken);

                    // Update the latest timestamp in the storage
                    _instagramDataStorage.UpdateLatestTimestamp(mediaList.Max(m => m.timestamp));

                    // Process the mediaList as needed
                    return Ok(mediaList);
                }

                return StatusCode(500, "Failed to retrieve Instagram data.");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
