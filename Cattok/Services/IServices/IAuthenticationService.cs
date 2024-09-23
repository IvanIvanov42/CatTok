using CatTok.Models;

namespace CatTok.Services.IServices
{
    public interface IAuthenticationService
    {
        Task InitializeAsync();

        event Action<string?>? LoginChange;
        ValueTask<string> GetJwtAsync();
        Task LoginAsync(LoginModel model);
        Task RegisterAsync(RegisterModel model);
        Task LogoutAsync();
        Task<bool> RefreshAsync();
    }
}