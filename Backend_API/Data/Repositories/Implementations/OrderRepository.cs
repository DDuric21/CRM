using Backend_API.Data.DbContext;
using Backend_API.Data.Model;

namespace Backend_API.Data.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(CrmDbContext context) : base(context)
        {
        }
    }
}
