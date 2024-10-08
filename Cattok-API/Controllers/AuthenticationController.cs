﻿using Azure.Security.KeyVault.Secrets;
using Cattok_API.Authentication;
using Cattok_API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace WebApiAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<InstagramUser> _userManager;
        private readonly SecretClient _secretClient;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(UserManager<InstagramUser> userManager, SecretClient secretClient, ILogger<AuthenticationController> logger)
        {
            _userManager = userManager;
            _secretClient = secretClient;
            _logger = logger;
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegistrationModel model)
        {
            _logger.LogInformation("Register called");

            var existingUser = await _userManager.FindByNameAsync(model.Username);

            if (existingUser != null)
                return Conflict("User already exists.");

            var newUser = new InstagramUser
            {
                Medias = new List<Media>(),
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Register succeeded");

                return Ok("User successfully created");
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError,
                       $"Failed to create user: {string.Join(" ", result.Errors.Select(e => e.Description))}");
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            _logger.LogInformation("Login called");

            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized();

            JwtSecurityToken token = GenerateJwt(user.Id, model.Username);

            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(1);

            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Login succeeded");

            return Ok(new LoginResponse
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("Refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh([FromBody] RefreshModel model)
        {
            _logger.LogInformation("Refresh called");

            var principal = GetPrincipalFromExpiredToken(model.AccessToken);

            if (principal?.Identity?.Name is null)
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user is null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
                return Unauthorized();

            var token = GenerateJwt(user.Id, principal.Identity.Name);

            _logger.LogInformation("Refresh succeeded");

            return Ok(new LoginResponse
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                RefreshToken = model.RefreshToken
            });
        }

        [Authorize]
        [HttpDelete("Revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Revoke()
        {
            _logger.LogInformation("Revoke called");

            var username = HttpContext.User.Identity?.Name;

            if (username is null)
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
                return Unauthorized();

            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Revoke succeeded");

            return Ok();
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var secret = GetSecret("JWT-Secret");

            var validation = new TokenValidationParameters
            {
                ValidIssuer = GetSecret("JWT-ValidIssuer"),
                ValidAudience = GetSecret("JWT-ValidAudience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateLifetime = false
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }

        private JwtSecurityToken GenerateJwt(string userId, string username)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                GetSecret("JWT-Secret")));

            var token = new JwtSecurityToken(
                issuer: GetSecret("JWT-ValidIssuer"),
                audience: GetSecret("JWT-ValidAudience"),
                expires: DateTime.UtcNow.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var generator = RandomNumberGenerator.Create();

            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private string GetSecret(string secretName)
        {
            KeyVaultSecret secret = _secretClient.GetSecret(secretName);
            return secret.Value;
        }
    }
}
