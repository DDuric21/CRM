using Models.DTO;

namespace UI.Services
{
    public interface ICustomerService
    {
        Task<IAsyncEnumerable<CustomerDTO>> GetCustomers();
    }
}
