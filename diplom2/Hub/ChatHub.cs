using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace diplom2.Hub
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task Send(string message)
        {
            await this.Clients.All.SendAsync("Send", message);
        }
    }
}
