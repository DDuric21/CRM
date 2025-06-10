using Backend_API.Data.Models;
using Models.DTO;
using Models.Requests;
using Models.Responses;

namespace Backend_API.Services
{
    public interface IOrderService
    {
        OrderDTO GetOrderData(Guid id);

        Task<ResponseBase> CreateNewOrderAsync(CreateOrderRQ createOrderRQ);

        Task<bool> SubmitOrderDataAsync(OrderDTO order);

        OrderDTO MapToDTO(Order order);
    }
}
