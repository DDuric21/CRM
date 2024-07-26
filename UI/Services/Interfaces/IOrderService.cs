using Models.DTO;

namespace UI.Services
{
    public interface IOrderService
    {
        Task<int> SubmitOrderAsync(OrderDTO orderDTO);

        Task<int> UpdateAsset(OrderDTO orderDTO);

        Task<int> DeleteAsset(long customerAssetID);

        Task CreateOrderAsync(Guid orderID, long customerID, long customerAssetID = 0);

        Task CreateOrderAsync(OrderDTO orderDTO, bool withOptions = false);

        Task<OrderDTO> GetOrderDataAsync(Guid id);
    }
}
