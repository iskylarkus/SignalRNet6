using SignalRNet6.API.Models;

namespace SignalRNet6.API.Hubs
{
    public interface IProductHub
    {
        Task ReceiveProduct(Product product);
    }
}
