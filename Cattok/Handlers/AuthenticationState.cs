namespace CatTok.Handlers
{
    public class AuthenticationState
    {
        private string? _username;
        public string? Username
        {
            get => _username;
            private set
            {
                if (_username != value)
                {
                    _username = value;
                    NotifyAuthenticationStateChanged();
                }
            }
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(Username);

        public event Action? AuthenticationStateChanged;

        public void SetUser(string? username)
        {
            Username = username;
        }

        private void NotifyAuthenticationStateChanged()
        {
            AuthenticationStateChanged?.Invoke();
        }
    }
}
