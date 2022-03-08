using Microsoft.AspNetCore.SignalR;
using SignalRNet6.API.Models;

namespace SignalRNet6.API.Hubs
{
    public class ProductHub:Hub<IProductHub>
    {
        public async Task SendProduct(Product product)
        {
            await Clients.All.ReceiveProduct(product);
        }
    }
}
