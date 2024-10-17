namespace CatTok.Services.IServices
{
    public interface IStreamingService
    {
        bool IsStreaming { get; }
        Task InitializeAsync();
        Task StartStreamingAsync();
        Task StopStreamingAsync();
    }
}
