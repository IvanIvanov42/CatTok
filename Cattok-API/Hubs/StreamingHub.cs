using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Cattok_API.Hubs
{
    [AllowAnonymous]
    public class StreamingHub : Hub 
    {
        private static readonly HashSet<string> ActiveStreamers = new HashSet<string>();

        public async Task StartStream()
        {
            ActiveStreamers.Add(Context.ConnectionId);
            await Clients.All.SendAsync("StreamerStarted", Context.ConnectionId);
        }

        public async Task StopStream()
        {
            ActiveStreamers.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("StreamerStopped", Context.ConnectionId);
        }
    }
}
