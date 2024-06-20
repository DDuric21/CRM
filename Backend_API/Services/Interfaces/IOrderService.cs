using Backend_API.Data.Model;
using Models.DTO;

namespace Backend_API.Services
{
    public interface IOrderService
    {
        Task CreateOrderAssetAsync(CustomerAssets customerAsset);

        Task CreateOrderAssetOptionsAsync(List<CustomerAssetOptions> customerAssetOptions);

        Task<int> DeleteCustomerAssetAsync(long customerAssetsID);

        CustomerAssets MapToCustomerAsset(OrderDTO orderDTO);

        List<CustomerAssetOptions> MapToCustomerAssetOptions(OrderDTO orderDTO, CustomerAssets customerAssets);
    }
}
