using Backend_API.Data.Model;
using Models.DTO;

namespace Backend_API.Services
{
    public interface IOrderService
    {
        Task CreateOrder(CustomerAssets customerAsset);
        CustomerAssets MapToOrderData(OrderDTO orderDTO);
    }
}
