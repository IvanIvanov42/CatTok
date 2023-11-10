
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

                // Deserialize the Instagram API response
                InstagramResponseModel? responseModel =
                JsonSerializer.Deserialize<InstagramResponseModel>(instagramData);
                foreach(var item in responseModel.data )
                {
                    listOfIds.Add(item.id);
                }

                // Process the Instagram data as needed
                return Ok(instagramData);
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
