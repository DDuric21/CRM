using Backend_API.Data.Models;
using Models.DTO;
using Models.Requests;
using Models.Responses;

namespace Backend_API.Services
{
    public interface IOrderService
    {
        Task<OrderDTO> GetOrderDataAsync(Guid id);

        Task<ResponseBase> CreateNewOrderAsync(CreateOrderRQ createOrderRQ);

        Task<ResponseBase> CancelOrderAsync(CancelOrderRQ cancelOrderRQ);

        Task<ResponseBase> SubmitOrderDataAsync(OrderDTO order);

        OrderDTO MapToDTO(Order order);

        Task<OrderGridFilterDataRS> GetOrderFilterBaseValuesAsync();

        Task<GetOrdersRS> GetOrdersAsync(OrderFilter orderFilter);
    }
}
