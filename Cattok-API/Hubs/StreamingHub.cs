using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Cattok_API.Hubs
{
    [AllowAnonymous]
    public class StreamingHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> ActiveStreams = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> Viewers = new ConcurrentDictionary<string, string>();

        public async Task StartStream(string streamerUserId)
        {
            if (ActiveStreams.Values.Contains(streamerUserId))
            {
                await Clients.Caller.SendAsync("StreamAlreadyActive");
                return;
            }

            ActiveStreams[Context.ConnectionId] = streamerUserId;
            Console.WriteLine($"Streamer started: UserId={streamerUserId}, ConnectionId={Context.ConnectionId}");
            await Clients.All.SendAsync("StreamStarted", streamerUserId);
        }

        public async Task StopStream(string streamerUserId)
        {
            ActiveStreams.TryRemove(Context.ConnectionId, out _);
            await Clients.All.SendAsync("StreamStopped", streamerUserId);
        }

        public async Task JoinStream(string streamerUserId)
        {
            var streamerConnectionId = ActiveStreams.FirstOrDefault(kvp => kvp.Value == streamerUserId).Key;
            if (streamerConnectionId != null)
            {
                // Map viewer to streamer
                Viewers[Context.ConnectionId] = streamerConnectionId;

                // Send a message to the streamer
                await Clients.Client(streamerConnectionId).SendAsync("ViewerJoined", Context.ConnectionId);
            }
            else
            {
                await Clients.Caller.SendAsync("StreamNotAvailable", streamerUserId);
            }
        }

        public async Task LeaveStream(string streamerUserId)
        {
            // Remove the viewer
            Viewers.TryRemove(Context.ConnectionId, out _);

            var streamerConnectionId = ActiveStreams.FirstOrDefault(kvp => kvp.Value == streamerUserId).Key;
            if (streamerConnectionId != null)
            {
                await Clients.Client(streamerConnectionId).SendAsync("ViewerLeft", Context.ConnectionId);
            }

            await Task.CompletedTask;
        }

        public Task<List<string>> GetActiveStreams()
        {
            var streams = ActiveStreams.Values.Distinct().ToList();
            return Task.FromResult(streams);
        }

        // Streamer sends offer to viewer
        public async Task SendOffer(string viewerConnectionId, string offer)
        {
            await Clients.Client(viewerConnectionId).SendAsync("ReceiveOffer", Context.ConnectionId, offer);
        }

        // Viewer sends answer to streamer
        public async Task SendAnswer(string streamerConnectionId, string answer)
        {
            if (ActiveStreams.ContainsKey(streamerConnectionId))
            {
                Console.WriteLine($"Sending answer to streamer: ConnectionId={streamerConnectionId}, ViewerConnectionId={Context.ConnectionId}");
                await Clients.Client(streamerConnectionId).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
            }
            else
            {
                Console.WriteLine($"Streamer not found for ConnectionId={streamerConnectionId}. Sending 'StreamerDisconnected' to viewer.");
                await Clients.Caller.SendAsync("StreamerDisconnected");
            }
        }

        // Streamers and viewers exchange ICE candidates
        public async Task SendIceCandidate(string connectionId, string candidate)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (ActiveStreams.TryRemove(Context.ConnectionId, out var streamerUserId))
            {
                Console.WriteLine($"Streamer disconnected: UserId={streamerUserId}, ConnectionId={Context.ConnectionId}");
                await Clients.All.SendAsync("StreamStopped", streamerUserId);

                // Streamer disconnected
                var viewers = Viewers.Where(kvp => kvp.Value == Context.ConnectionId).Select(kvp => kvp.Key).ToList();
                foreach (var viewerConnectionId in viewers)
                {
                    await Clients.Client(viewerConnectionId).SendAsync("StreamerDisconnected");
                    Viewers.TryRemove(viewerConnectionId, out _);
                }
            }
            else
            {
                // Viewer disconnected
                if (Viewers.TryRemove(Context.ConnectionId, out var streamerConnectionId))
                {
                    await Clients.Client(streamerConnectionId).SendAsync("ViewerLeft", Context.ConnectionId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
