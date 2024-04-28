using Backend_API.Data.Model;

namespace Backend_API.Data.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<int> UpdateCustomerAsync(Customer customer);
        Task<List<Customer>> GetAllCustomersAsync();
    }
}
