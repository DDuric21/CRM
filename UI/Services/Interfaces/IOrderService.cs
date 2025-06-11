using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public interface IOrderService
    {
        Task<bool> SubmitOrderAsync(OrderDTO orderDTO);

        Task<ResponseBase> CreateOrderAsync(OrderDTO orderDTO, bool withOptions = false);

        Task<OrderDTO> GetOrderDataAsync(Guid id);

        Task<ResponseBase> CancelOrderAsync(Guid id);
    }
}
