using Backend_API.Data.Model;
using Models.DTO;
using Models.Enums;

namespace Backend_API.Services
{
    public interface IOrderService
    {
        Task<int> SubmitOrderAsync(Order order);

        void MapOptionsToOrderAsset(OrderDTO orderDTO);

        Task<int> UpdateOrderStatusAsync(Order order, int orderStatusID);

        CrudAction DefineOrderAction(CrudAction crudAction);

        Task CreateOrderAssetOptionsAsync(List<CustomerAssetOptions> customerAssetOptions);

        Task<int> DeactivateCustomerAssetAsync(Order order);

        Task<int> UpdateOrderStatusAsync(Guid id, int orderStatusID);

        Task<int> UpdateCustomerAssetAsync(CustomerAssets customerAssets, Guid orderID);

        Order GetOrderData(Guid id);

        CustomerAssets MapToCustomerAsset(OrderDTO orderDTO);

        List<CustomerAssetOptions> MapToCustomerAssetOptions(OrderDTO orderDTO, CustomerAssets customerAssets);

        CustomerAssets MapToCustomerAssetData(OrderDTO orderDTO);

        Order MapDtoToOrder(OrderDTO orderDTO, bool withOptions = false);

        Task CreateOrderAsync(Order order);

        OrderDTO MapToDTO(Order order);

        CustomerAssets MapToCustomerAssetBasicData(OrderDTO orderDTO);
    }
}
