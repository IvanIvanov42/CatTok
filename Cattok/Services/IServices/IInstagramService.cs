namespace CatTok.Services.IServices
{
    public interface IInstagramService
    {
        Task<HttpResponseMessage> SendAuthorizationToken(string token);

        string GetUser();
    }
}