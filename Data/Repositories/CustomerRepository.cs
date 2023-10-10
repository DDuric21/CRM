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
    }
}
