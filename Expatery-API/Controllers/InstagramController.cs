
using Expatery_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Expatery_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstagramController : ControllerBase
    {
        private readonly InstagramService _instagramService;

        public InstagramController(InstagramService instagramService)
        {
            _instagramService = instagramService;
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

            // Use the access token in your InstagramService
            try
            {
                List<string> listOfIds = new List<string>();
                var instagramData = await _instagramService.GetInstagramDataAsync(accessToken);

                InstagramResponseModel? responseModel = JsonSerializer.Deserialize<InstagramResponseModel>(instagramData);

                if (responseModel != null)
                {
                    listOfIds.AddRange(responseModel.data.Select(item => item.id));

                    // Call the new method to get media details
                    List<Media> mediaList = await _instagramService.GetMediaDetailsAsync(listOfIds, accessToken);

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
