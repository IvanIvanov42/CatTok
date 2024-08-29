using Azure.Security.KeyVault.Secrets;
using Cattok_API.Authentication;
using Cattok_API.Data.Models;
using Cattok_API.Data.Repository;
using Cattok_API.Models;
using Cattok_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cattok_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InstagramController : ControllerBase
    {
        private readonly UserManager<InstagramUser> _userManager;
        private readonly IInstagramService _instagramService;
        private readonly IMediaRepository _dataStorage;

        public InstagramController(UserManager<InstagramUser> userManager, IInstagramService instagramService, IMediaRepository dataStorage)
        {
            _userManager = userManager;
            _instagramService = instagramService;
            _dataStorage = dataStorage;
        }

        [HttpGet("GetInstagramData/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetInstagramData(string userId)
        {
            try
            {
                var mediaList = await _dataStorage.GetMediaListAsync(userId);

                if (mediaList != null)
                {
                    return Ok(mediaList);
                }

                return StatusCode(500, "Failed to retrieve media data from the database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostInstagramData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // get user token
            var token = user.InstagramToken;

            if (string.IsNullOrEmpty(token))
            {
                // Handle the case where the access token is not available
                return StatusCode(500, "Instagram access token not found.");
            }

            try
            {
                var instagramData = await _instagramService.GetInstagramDataAsync(token);
                await _dataStorage.AddMediaAsync(userId, instagramData);
                return Ok("Data posted successfully");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("AuthorizeUser")]
        public async Task<IActionResult> AuthorizeUser([FromBody] string token)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Authorization code is required.");
            }

            try
            {
                var accessToken = await _instagramService.GetShortLivedAccessTokenAsync(token);

                if (string.IsNullOrEmpty(accessToken))
                {
                    return Unauthorized("Could not obtain access token.");
                }

                var longLivedToken = await _instagramService.GetLongLivedAccessTokenAsync(accessToken);

                if (string.IsNullOrEmpty(longLivedToken))
                {
                    return Unauthorized("Could not convert to long-lived token.");
                }

                user.InstagramToken = longLivedToken;
                user.InstagramTokenExpiry = DateTime.UtcNow.AddDays(60);

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return StatusCode(500, "Could not update user with token.");
                }

                return Ok(new { AccessToken = longLivedToken });
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