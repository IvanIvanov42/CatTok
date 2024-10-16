using CatTok.Services.IServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace CatTok.Services
{
    public class StreamingService : IStreamingService
    {
        private readonly IJSRuntime jsRuntime;
        private HubConnection hubConnection;
        private readonly IConfiguration configuration;
        private readonly ILogger<StreamingService> logger;

        public StreamingService(
            IConfiguration configuration, IJSRuntime jSRuntime, ILogger<StreamingService> logger)
        {
            this.configuration = configuration;
            this.jsRuntime = jSRuntime;
            this.logger = logger;
        }

        public async Task InitializeAsync()
        {
            if (hubConnection != null)
                return;

            var serverUrl = configuration["ServerUrl"];
            var hubUrl = $"{serverUrl}streaminghub";

            hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();

            hubConnection.On<string>("TestMessage", TestMessage);

            try
            {
                await hubConnection.StartAsync();
                logger.LogInformation("Hub connection started.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error starting hub connection.");
            }
        }

        public async Task TestStream()
        {
            await hubConnection.InvokeAsync("SendMessage", "testUser", "testMessage");
        }

        public async Task TestMessage(string message)
        {
            logger.LogInformation($"Received message from server: {message}");
        }
    }
}
