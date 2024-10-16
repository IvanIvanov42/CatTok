namespace CatTok.Services.IServices
{
    public interface IStreamingService
    {
        Task InitializeAsync();

        Task TestStream();
    }
}
