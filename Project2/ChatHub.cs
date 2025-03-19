using Microsoft.AspNetCore.SignalR;

namespace Project2
{
    public class ChatHub : Hub
    {
        public async Task Reload()
        {
            await Clients.All.SendAsync("Update");
        }
    }
}
