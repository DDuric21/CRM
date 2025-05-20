using Models.DTO;

namespace UI.Services
{
    public interface IOrderService
    {
        Task<bool> SubmitOrderAsync(OrderDTO orderDTO);

        Task<bool> CreateOrderAsync(OrderDTO orderDTO, bool withOptions = false);

        Task<OrderDTO> GetOrderDataAsync(Guid id);
    }
}
