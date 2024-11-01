namespace CatTok.Services.IServices
{
    public interface IStreamingService
    {
        bool IsStreaming { get; }
        string CurrentStreamerId { get; }

        Task InitializeAsync();
        Task StartStreaming(string userId, bool audio);
        Task StopStreaming(string userId);

        Task<List<string>> GetActiveStreamsAsync();
        Task JoinStreamAsync(string streamerId);
        Task LeaveStreamAsync();
        Task SendIceCandidateAsync(string connectionId, string candidate);
        Task SendAnswerAsync(string streamerId, string answer);

        event Action<List<string>> OnActiveStreamsUpdated;
        event Action OnStreamingStateChanged;
    }
}
