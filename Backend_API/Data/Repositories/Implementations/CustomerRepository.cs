using Backend_API.Data.DbContext;
using Backend_API.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend_API.Data.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var customer = await _context.Customers
                .Include(x => x.Addresses)
                .ToListAsync();

            return customer;
        }

        public async Task<int> UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = _context.Customers
                .Include(c => c.Addresses)
                .SingleOrDefault(c => c.Id == customer.Id);

            if (existingCustomer == null)
            {
                return 0;
            }

            _context.Entry(existingCustomer).CurrentValues.SetValues(customer);

            foreach (var address in existingCustomer.Addresses)
            {
                _context.Entry(address).State = EntityState.Modified;
            }

            return await SaveAsync();
        }

        public async Task<Customer> GetAllCustomerRelatedDataAsync(long customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Addresses)
                .Include(c => c.BillingProfiles)
                    .ThenInclude(bp => bp.Address)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            return customer;
        }
    }
}
