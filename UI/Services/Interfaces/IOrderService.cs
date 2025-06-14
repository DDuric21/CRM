using Models.DTO;
using Models.Requests;
using Models.Responses;
using UI.Helpers;

namespace UI.Services
{
    public interface IOrderService
    {
        Task<ResponseBase> SubmitOrderAsync(OrderDTO orderDTO);

        Task<ResponseBase> CreateOrderAsync(OrderDTO orderDTO, bool withOptions = false);

        Task<OrderDTO> GetOrderDataAsync(Guid id);

        Task<ResponseBase> CancelOrderAsync(Guid id);

        Task<ActionResult<OrderGridFilterDataRS>> GetOrderFilterBaseValuesAsync();

        Task<ActionResult<GetOrdersRS>> GetOrdersAsync(OrderFilterRQ orderFilterRQ);
    }
}
