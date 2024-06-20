using Backend_API.Data.DbContext;
using Backend_API.Data.Model;

namespace Backend_API.Data.Repositories
{
    public class CustomerAssetOptionsRepository : GenericRepository<CustomerAssetOptions>, ICustomerAssetOptionsRepository
    {
        public CustomerAssetOptionsRepository(CrmDbContext context) : base(context)
        {
        }
    }
}
