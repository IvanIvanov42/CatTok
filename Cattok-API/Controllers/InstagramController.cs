using Azure.Security.KeyVault.Secrets;
using Cattok_API.Data.Models;
using Cattok_API.Data.Repository;
using Cattok_API.Models;
using Cattok_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cattok_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InstagramController : ControllerBase
    {
        private readonly IInstagramService _instagramService;
        private readonly IMediaRepository _dataStorage;
        private readonly SecretClient _secretClient;

        public InstagramController(IInstagramService instagramService, IMediaRepository dataStorage, SecretClient secretClient)
        {
            _instagramService = instagramService;
            _dataStorage = dataStorage;
            _secretClient = secretClient;
        }

        [HttpGet("GetInstagramData")]
		[AllowAnonymous]
        public async Task<IActionResult> GetInstagramData()
        {
            try
            {
                // Call the method in your service to get media data from the database
                var mediaList = await _dataStorage.GetMediaListAsync();

                if (mediaList != null)
                {
                    // Process the mediaList as needed
                    return Ok(mediaList);
                }

                return StatusCode(500, "Failed to retrieve media data from the database.");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetSecrets")]
        public async Task<IActionResult> GetSecrets()
        {
            try
            {
                KeyVaultSecret secretClientId = await _secretClient.GetSecretAsync("FEClientId");
                KeyVaultSecret secretRedirectUri = await _secretClient.GetSecretAsync("FERedirectUri");

                var response = new SecretResponse
                {
                    ClientId = secretClientId.Value,
                    RedirectUri = secretRedirectUri.Value
                };

                return Ok(response);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostInstagramData([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                // Handle the case where the access token is not available
                return StatusCode(500, "Instagram access token not found.");
            }
            try
            {
                var instagramData = await _instagramService.GetInstagramDataAsync(token);

                if (instagramData.Count > 0)
                {
                    try
                    {
                        await _dataStorage.AddMediaAsync(instagramData);

                        return Ok("Data posted successfully");
                    }
                    catch (Exception ex)
                    {
                        // Log the exception
                        Console.WriteLine(ex);
                        return StatusCode(500, "Internal Server Error");
                    }
                }
                return StatusCode(204, "No new data.");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("AuthorizeUser")]
        public async Task<IActionResult> AuthorizeUser([FromBody]string token)
        {
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

                // Optionally store the long-lived token in the database or some secure storage for future use.

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