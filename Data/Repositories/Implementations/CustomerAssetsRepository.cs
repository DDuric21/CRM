using Backend_API.Data.DbContext;
using Backend_API.Data.Model;

namespace Backend_API.Data.Repositories
{
    public class CustomerAssetsRepository : GenericRepository<CustomerAssets>, ICustomerAssetsRepository
    {
        public CustomerAssetsRepository(CrmDbContext context) : base(context)
        {
        }
    }
}
