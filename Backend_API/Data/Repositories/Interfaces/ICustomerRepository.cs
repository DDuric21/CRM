using Backend_API.Data.Models;

namespace Backend_API.Data.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<int> UpdateCustomerAsync(Customer customer);

        Task<Customer> GetAllCustomerRelatedDataAsync(long customerId);
    }
}
