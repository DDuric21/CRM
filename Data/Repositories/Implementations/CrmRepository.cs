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

        private IAddressRepository _addresses;

        public IAddressRepository Addresses
        {
            get
            {
                if (_addresses == null)
                {
                    _addresses = new AddressRepository(_context);
                }

                return _addresses;
            }
        }

        private IAssetRepository _assets;

        public IAssetRepository Assets
        {
            get
            {
                if (_assets == null)
                {
                    _assets = new AssetRepository(_context);
                }

                return _assets;
            }
        }

        private ICustomerAssetsRepository _customerAssets;

        public ICustomerAssetsRepository CustomerAssets
        {
            get
            {
                if (_customerAssets == null)
                {
                    _customerAssets = new CustomerAssetsRepository(_context);
                }

                return _customerAssets;
            }
        }

        private IOptionRepository _options;

        public IOptionRepository Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new OptionRepository(_context);
                }

                return _options;
            }
        }

        private IUserRepository _users;

        public IUserRepository Users
        {
            get
            {
                if (_users == null)
                {
                    _users = new UserRepository(_context);
                }

                return _users;
            }
        }

    }
}
