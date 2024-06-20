using Models.DTO;

namespace UI.Services
{
    public interface IOrderService
    {
        Task AddAsset(OrderDTO orderDTO);

        Task UpdateAsset(OrderDTO orderDTO);

        Task<int> DeleteAsset(long customerAssetID);
    }
}
