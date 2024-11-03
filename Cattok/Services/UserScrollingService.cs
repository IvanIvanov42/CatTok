using CatTok.Models;
using CatTok.Services.IServices;

namespace CatTok.Services
{
    public class UserScrollingService : IUserScrollingService
    {
        public event Action? OnUserChanged;
        private List<InstagramUser>? Users;
        private int CurrentUserIndex = 0;
        public InstagramUser? SelectedUser { get; private set; }
        public bool HasNextUser => Users != null && CurrentUserIndex < Users.Count - 1;
        public bool HasPreviousUser => Users != null && CurrentUserIndex > 0;

        private readonly IInstagramService _instagramService;

        public UserScrollingService(IInstagramService instagramService)
        {
            _instagramService = instagramService;
        }

        public async Task InitializeAsync()
        {
            var users = await _instagramService.GetUsersWithMediaAsync();
            if (users != null && users.Any())
            {
                Users = users.ToList();
                SelectedUser = Users[CurrentUserIndex];
                OnUserChanged?.Invoke();
            }
        }

        public bool LoadNextUser()
        {
            if (HasNextUser)
            {
                CurrentUserIndex++;
                SelectedUser = Users[CurrentUserIndex];
                OnUserChanged?.Invoke();
                return true;
            }
            return false;
        }

        public bool LoadPreviousUser()
        {
            if (HasPreviousUser)
            {
                CurrentUserIndex--;
                SelectedUser = Users[CurrentUserIndex];
                OnUserChanged?.Invoke();
                return true;
            }
            return false;
        }
    }
}
