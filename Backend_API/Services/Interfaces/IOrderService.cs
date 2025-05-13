using Backend_API.Data.Models;
using Models.DTO;
using Models.Requests;

namespace Backend_API.Services
{
    public interface IOrderService
    {
        OrderDTO GetOrderData(Guid id);

        Task<bool> CreateNewOrderAsync(CreateOrderRQ createOrderRQ);

        Task<bool> SubmitOrderDataAsync(OrderDTO order);

        OrderDTO MapToDTO(Order order);
    }
}
