using CatTok.Models;

namespace CatTok.Services.IServices
{
    public interface IUserScrollingService
    {
        bool HasNextUser { get; }
        bool HasPreviousUser { get; }
        InstagramUser? SelectedUser { get; }

        event Action? OnUserChanged;

        Task InitializeAsync();
        bool LoadNextUser();
        bool LoadPreviousUser();
    }
}