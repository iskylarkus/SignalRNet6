using Microsoft.AspNetCore.SignalR;

namespace SignalRNet6.CovidChart.API.Hubs
{
    public class CovidHub:Hub
    {
        public async Task GetCovidList()
        {
            await Clients.All.SendAsync("ReceiveCovidList", "get all values from service");
        }
    }
}
