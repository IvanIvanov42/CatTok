﻿using Blazored.LocalStorage;
using CatTok.Handlers;
using CatTok.Models;
using CatTok.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;

namespace CatTok.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILocalStorageService _localStorageService;
        private readonly AuthenticationState _authenticationState;

        private const string JWT_KEY = nameof(JWT_KEY);
        private const string REFRESH_KEY = nameof(REFRESH_KEY);

        private string? _jwtCache;

        public event Action<string?>? LoginChange;

        public AuthenticationService(
            IHttpClientFactory factory,
            ILocalStorageService localStorageService,
            AuthenticationState authenticationState)
        {
            _factory = factory;
            _localStorageService = localStorageService;
            _authenticationState = authenticationState;
        }

        public async Task InitializeAsync()
        {
            var jwt = await _localStorageService.GetItemAsync<string>(JWT_KEY);
            if (!string.IsNullOrEmpty(jwt))
            {
                var username = GetUsername(jwt);
                _authenticationState.SetUser(username);
                await _localStorageService.SetItemAsStringAsync("username", username);
                LoginChange?.Invoke(username);
            }
        }

        public async ValueTask<string> GetJwtAsync()
        {
            if (string.IsNullOrEmpty(_jwtCache))
                _jwtCache = await _localStorageService.GetItemAsync<string>(JWT_KEY);

            return _jwtCache;
        }

        public async Task LogoutAsync()
        {
            var response = await _factory.CreateClient("CatTokAPI").DeleteAsync("api/Authentication/Revoke");

            await _localStorageService.RemoveItemAsync(JWT_KEY);
            await _localStorageService.RemoveItemAsync(REFRESH_KEY);

            _jwtCache = null;

            _authenticationState.SetUser(null);

            await Console.Out.WriteLineAsync($"Revoke gave response {response.StatusCode}");

            LoginChange?.Invoke(null);
        }

        private static string GetUsername(string token)
        {
            var jwt = new JwtSecurityToken(token);
            return jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        }

        public async Task LoginAsync(LoginModel model)
        {
            var response = await _factory.CreateClient("CatTokAPI")
                .PostAsync("api/Authentication/Login", JsonContent.Create(model));

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Login failed.");

            var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (content == null)
                throw new InvalidDataException("Invalid response from login.");

            await _localStorageService.SetItemAsync(JWT_KEY, content.JwtToken);
            await _localStorageService.SetItemAsync(REFRESH_KEY, content.RefreshToken);

            var username = GetUsername(content.JwtToken);
            _authenticationState.SetUser(username);

            LoginChange?.Invoke(username);
        }


        public async Task RegisterAsync(RegisterModel model)
        {
            var response = await _factory.CreateClient("CatTokAPI")
                .PostAsync("api/Authentication/Register", JsonContent.Create(model));

            if (response.StatusCode == HttpStatusCode.Conflict)
                throw new Exception("User already exists.");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Registration failed.");

            // Automatically login
            await LoginAsync(new LoginModel { Username = model.Username, Password = model.Password });
        }

        public async Task<bool> RefreshAsync()
        {
            var model = new RefreshModel
            {
                AccessToken = await _localStorageService.GetItemAsync<string>(JWT_KEY),
                RefreshToken = await _localStorageService.GetItemAsync<string>(REFRESH_KEY)
            };

            var response = await _factory.CreateClient("CatTokAPI").PostAsync("api/Authentication/Refresh",
                                                        JsonContent.Create(model));

            if (!response.IsSuccessStatusCode)
            {
                await LogoutAsync();
                return false;
            }

            var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (content == null)
                throw new InvalidDataException();

            await _localStorageService.SetItemAsync(JWT_KEY, content.JwtToken);
            await _localStorageService.SetItemAsync(REFRESH_KEY, content.RefreshToken);

            _jwtCache = content.JwtToken;
            _authenticationState.SetUser(GetUsername(content.JwtToken));

            return true;
        }
    }
}
