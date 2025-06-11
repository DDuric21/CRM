using Models.DTO;
using Models.Requests;
using Models.Responses;

namespace UI.Services
{
    public interface ICustomerService
    {
        Task<IAsyncEnumerable<CustomerDTO>> GetCustomersAsync(CustomerFilterRQ customerFilter);

        Task<long> CreateNewCustomerAsync(CustomerDTO customerDTO);

        Task<CustomerDTO> GetCustomerDataAsync(long customerID);

        Task<bool> DeleteCustomer(long customerID);

        Task<bool> UpdateCustomer(CustomerDTO customerDTO);

        Task<IAsyncEnumerable<AssetDTO>> GetCustomerAssetsAsync(long id);

        Task<AssetDTO> GetCustomerAssetDataAsync(long customerAssetid);

        Task<IEnumerable<OrderDTO>> GetCustomerOrdersAsync(long customerID);

        Task<IAsyncEnumerable<InteractionDTO>> GetCustomerInteractionsAsync(long customerID);

        Task<CustomerGridFilterDataRS> GetCustomerFilterBaseValues();
    }
}
