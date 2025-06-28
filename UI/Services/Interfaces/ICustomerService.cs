using Models.DTO;
using Models.Requests;
using Models.Responses;
using UI.Helpers;

namespace UI.Services
{
    public interface ICustomerService
    {
        Task<IAsyncEnumerable<CustomerDTO>> GetCustomersAsync(CustomerFilterRQ customerFilter);

        Task<long> CreateNewCustomerAsync(CustomerDTO customerDTO);

        Task<ActionResult<CustomerDTO>> GetCustomerDataAsync(long customerID);

        Task<bool> DeleteCustomer(long customerID);

        Task<ResponseBase> UpdateCustomer(CustomerDTO customerDTO);

        Task<IAsyncEnumerable<AssetDTO>> GetCustomerAssetsAsync(long id);

        Task<AssetDTO> GetCustomerAssetDataAsync(long customerAssetid);

        Task<IEnumerable<OrderDTO>> GetCustomerOrdersAsync(long customerID);

        Task<IAsyncEnumerable<InteractionDTO>> GetCustomerInteractionsAsync(long customerID);

        Task<CustomerGridFilterDataRS> GetCustomerFilterBaseValues();
    }
}
