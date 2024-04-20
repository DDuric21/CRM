using Backend_API.Data.DbContext;
using Backend_API.Data.Model;

namespace Backend_API.Data.Repositories
{
    public class CustomerAddressesRepository : GenericRepository<CustomerAddresses>, ICustomerAddressesRepository
    {
        public CustomerAddressesRepository(CrmDbContext context) : base(context)
        {
        }
    }
}
