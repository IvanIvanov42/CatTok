using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Cattok_API.Hubs
{
    [AllowAnonymous]
    public class StreamingHub : Hub 
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("Client connected: " + Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)  
        {
            Console.WriteLine($"Received message from {user}: {message}");
            await Clients.All.SendAsync("TestMessage", message);
        }
    }
}
