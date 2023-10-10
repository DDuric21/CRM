using Backend_API.Data.DbContext;

namespace Backend_API.Data.Repositories
{
    public class CrmRepository : ICrmRepository
    {
        private readonly CrmDbContext _context;
        public CrmRepository(CrmDbContext context)
        {
            _context = context;
        }

        private ICustomerRepository _customers;

        public ICustomerRepository Customers
        {
            get
            {
                if (_customers == null)
                {
                    _customers = new CustomerRepository(_context);
                }

                return _customers;
            }
        }

    }
}
