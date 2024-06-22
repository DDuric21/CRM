using Backend_API.Data.Model;
using Models.DTO;

namespace Backend_API.Services
{
    public interface IOrderService
    {
        Task SubmitOrderAsync(CustomerAssets customerAsset);

        Task CreateOrderAssetOptionsAsync(List<CustomerAssetOptions> customerAssetOptions);

        Task<int> DeleteCustomerAssetAsync(long customerAssetsID);

        Task<int> UpdateCustomerAssetAsync(CustomerAssets customerAssets);

        CustomerAssets MapToCustomerAsset(OrderDTO orderDTO);

        List<CustomerAssetOptions> MapToCustomerAssetOptions(OrderDTO orderDTO, CustomerAssets customerAssets);

        CustomerAssets MapToCustomerAssetData(OrderDTO orderDTO);

        Order MapDtoToOrder(OrderDTO orderDTO);

        Task CreateOrderAsync(Order order);
    }
}
