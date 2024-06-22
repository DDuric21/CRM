using Models.DTO;

namespace UI.Services
{
    public interface IOrderService
    {
        Task AddAsset(OrderDTO orderDTO);

        Task<int> UpdateAsset(OrderDTO orderDTO);

        Task<int> DeleteAsset(long customerAssetID);

        Task CreateOrderAsync(Guid orderID, long customerID, long customerAssetID = 0);
    }
}
