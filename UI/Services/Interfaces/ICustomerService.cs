using Models.DTO;

namespace UI.Services
{
    public interface ICustomerService
    {
        Task<IAsyncEnumerable<CustomerDTO>> GetCustomersAsync();

        Task<long> CreateNewCustomerAsync(CustomerDTO customerDTO);

        Task<CustomerDTO> GetCustomerDataAsync(long customerID);

        Task DeleteCustomer(long customerID);

        Task UpdateCustomer(CustomerDTO customerDTO);
    }
}
