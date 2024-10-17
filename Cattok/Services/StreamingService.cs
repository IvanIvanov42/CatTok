using CatTok.Services.IServices;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace CatTok.Services
{
    public class StreamingService : IStreamingService
    {
        private HubConnection hubConnection;
        private DotNetObjectReference<StreamingService> objRef;

        public bool IsStreaming { get; private set; } = false;
        public event Action OnStreamingStateChanged;

        public string CurrentStreamerId { get; private set; }

        public event Action<List<string>> OnActiveStreamsUpdated;

        private List<string> activeStreams = new List<string>();

        private readonly IJSRuntime jsRuntime;
        private readonly IConfiguration configuration;
        private readonly ILogger<StreamingService> logger;

        public StreamingService(
            IJSRuntime jsRuntime,
            IConfiguration configuration,
            ILogger<StreamingService> logger)
        {
            this.jsRuntime = jsRuntime;
            this.configuration = configuration;
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

            // Register event handlers
            hubConnection.On<string>("ViewerJoined", OnViewerJoined);
            hubConnection.On<string, string>("ReceiveAnswer", OnReceiveAnswer);
            hubConnection.On<string, string>("ReceiveIceCandidate", OnReceiveIceCandidate);
            hubConnection.On("StreamAlreadyActive", OnStreamAlreadyActive);

            hubConnection.On<string>("StreamStarted", OnStreamStarted);
            hubConnection.On<string>("StreamStopped", OnStreamStopped);
            hubConnection.On<string, string>("ReceiveOffer", OnReceiveOffer);
            hubConnection.On<string, string>("ReceiveIceCandidate", OnReceiveIceCandidateForViewer);
            hubConnection.On("StreamerDisconnected", OnStreamerDisconnected);
            hubConnection.On<string>("StreamNotAvailable", OnStreamNotAvailable);

            hubConnection.Closed += async (error) =>
            {
                logger.LogWarning("Connection closed. Attempting to reconnect...");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                try
                {
                    await hubConnection.StartAsync();
                    logger.LogInformation("Reconnected.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error reconnecting.");
                }
            };

            try
            {
                await hubConnection.StartAsync();
                logger.LogInformation("Hub connection started.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error starting hub connection.");
            }

            // Initialize JavaScript interop reference
            objRef = DotNetObjectReference.Create(this);
            await jsRuntime.InvokeVoidAsync("streamingFunctions.setDotNetReference", objRef);

            // Get the initial list of active streams
            activeStreams = await hubConnection.InvokeAsync<List<string>>("GetActiveStreams");
            OnActiveStreamsUpdated?.Invoke(activeStreams);
        }

        public async Task StartStreaming(string userId)
        {
            await InitializeAsync();

            try
            {
                bool started = await jsRuntime.InvokeAsync<bool>("streamingFunctions.startStreaming");
                logger.LogInformation($"startStreaming returned: {started}");

                if (!started)
                {
                    IsStreaming = false;
                    OnStreamingStateChanged?.Invoke();
                    return;
                }

                await hubConnection.InvokeAsync("StartStream", userId);
                IsStreaming = true;
                OnStreamingStateChanged?.Invoke();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error starting streaming.");
                IsStreaming = false;
                OnStreamingStateChanged?.Invoke();
            }
        }

        public async Task StopStreaming(string userId)
        {
            if (!IsStreaming)
                return;

            try
            {
                await hubConnection.InvokeAsync("StopStream", userId);
                await jsRuntime.InvokeVoidAsync("streamingFunctions.stopStreaming");
                IsStreaming = false;
                OnStreamingStateChanged?.Invoke();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error stopping streaming.");
            }
        }

        // Viewer methods

        public async Task<List<string>> GetActiveStreamsAsync()
        {
            if (hubConnection == null)
                await InitializeAsync();

            activeStreams = await hubConnection.InvokeAsync<List<string>>("GetActiveStreams");
            return activeStreams;
        }

        public async Task JoinStreamAsync(string streamerId)
        {
            if (hubConnection == null)
                await InitializeAsync();

            await hubConnection.InvokeAsync("JoinStream", streamerId);
            CurrentStreamerId = streamerId;
        }

        public async Task LeaveStreamAsync()
        {
            if (CurrentStreamerId != null)
            {
                await hubConnection.InvokeAsync("LeaveStream", CurrentStreamerId);
                CurrentStreamerId = null;
            }
        }

        public async Task SendIceCandidateAsync(string connectionId, string candidate)
        {
            await hubConnection.InvokeAsync("SendIceCandidate", connectionId, candidate);
        }

        public async Task SendAnswerAsync(string streamerConnectionId, string answer)
        {
            await hubConnection.InvokeAsync("SendAnswer", streamerConnectionId, answer);
        }

        // JavaScript invokable methods
        [JSInvokable]
        public async Task SendIceCandidate(string connectionId, string candidate)
        {
            await SendIceCandidateAsync(connectionId, candidate);
        }

        // Event handlers for SignalR hub messages
        private async Task OnViewerJoined(string viewerConnectionId)
        {
            logger.LogInformation($"Viewer joined: {viewerConnectionId}");
            try
            {
                string offer = await jsRuntime.InvokeAsync<string>("streamingFunctions.createOffer", viewerConnectionId);
                await hubConnection.InvokeAsync("SendOffer", viewerConnectionId, offer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling OnViewerJoined.");
            }
        }

        private async Task OnReceiveAnswer(string viewerConnectionId, string answer)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("streamingFunctions.receiveAnswer", viewerConnectionId, answer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling OnReceiveAnswer.");
            }
        }

        private async Task OnReceiveIceCandidate(string connectionId, string candidate)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("streamingFunctions.addIceCandidate", connectionId, candidate);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling OnReceiveIceCandidate.");
            }
        }

        private void OnStreamAlreadyActive()
        {
            logger.LogWarning("Stream is already active.");
        }

        private void OnStreamStarted(string streamerId)
        {
            if (!activeStreams.Contains(streamerId))
            {
                activeStreams.Add(streamerId);
                OnActiveStreamsUpdated?.Invoke(activeStreams);
            }
        }

        private void OnStreamStopped(string streamerId)
        {
            if (activeStreams.Contains(streamerId))
            {
                activeStreams.Remove(streamerId);
                OnActiveStreamsUpdated?.Invoke(activeStreams);

                if (CurrentStreamerId == streamerId)
                {
                    _ = DisposeViewingResourcesAsync();
                }
            }
        }

        private async Task DisposeViewingResourcesAsync()
        {
            try
            {
                await LeaveStreamAsync();
                // Clear the remote video element
                await jsRuntime.InvokeVoidAsync("streamingFunctions.clearRemoteVideo");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error disposing viewing resources.");
            }
        }

        private async Task OnReceiveOffer(string streamerConnectionId, string offer)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("streamingFunctions.receiveOffer", streamerConnectionId, offer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling OnReceiveOffer.");
            }
        }

        private async Task OnReceiveIceCandidateForViewer(string connectionId, string candidate)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("streamingFunctions.addIceCandidate", connectionId, candidate);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling OnReceiveIceCandidate for viewer.");
            }
        }

        private void OnStreamerDisconnected()
        {
            logger.LogWarning("Streamer disconnected.");
        }

        private void OnStreamNotAvailable(string streamerId)
        {
            logger.LogWarning($"Stream not available for streamer: {streamerId}");
        }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection != null)
            {
                try
                {
                    await hubConnection.DisposeAsync();
                    hubConnection = null;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error disposing hub connection.");
                }
            }

            if (objRef != null)
            {
                objRef.Dispose();
                objRef = null;
            }
        }
    }
}
