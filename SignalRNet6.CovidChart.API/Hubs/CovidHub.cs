using Microsoft.AspNetCore.SignalR;
using SignalRNet6.CovidChart.API.Models;

namespace SignalRNet6.CovidChart.API.Hubs
{
    public class CovidHub:Hub
    {
        private readonly CovidService _service;

        public CovidHub(CovidService service)
        {
            _service = service;
        }

        public async Task GetCovidList()
        {
            await Clients.All.SendAsync("ReceiveCovidList", _service.GetCovidChart());
        }
    }
}
