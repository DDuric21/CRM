using Models.DTO;

namespace CRM_UI.Services
{
    public interface ICustomerService
    {
        Task<IAsyncEnumerable<CustomerDTO>> GetCustomers();
    }
}
