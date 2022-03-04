using Microsoft.AspNetCore.SignalR;

namespace SignalRNet6.API.Hubs
{
    public class MyHub:Hub
    {
        private static List<string> Names { get; set; } = new List<string>();
        private static int ClientCount { get; set; } = 0;
        public static int TeamCount { get; set; } = 7;

        public async Task SendName(string name)
        {
            if (TeamCount > Names.Count)
            {
                Names.Add(name);

                await Clients.All.SendAsync("ReceiveName", name);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", $"Our team must be no more than {TeamCount} members..!");
            }
        }

        public async Task GetNames()
        {
            await Clients.All.SendAsync("ReceiveNames", Names);
        }

        public override async Task OnConnectedAsync()
        {
            ClientCount++;

            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ClientCount--;

            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
