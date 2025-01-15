using Backend_API.Data.DbContext;
using Backend_API.Data.Model;

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

        private ICustomerAssetOptionsRepository _customerAssetOptions;

        public ICustomerAssetOptionsRepository CustomerAssetOptions
        {
            get
            {
                if (_customerAssetOptions == null)
                {
                    _customerAssetOptions = new CustomerAssetOptionsRepository(_context);
                }

                return _customerAssetOptions;
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

        private IOrderRepository _orders;

        public IOrderRepository Orders
        {
            get
            {
                if (_orders == null)
                {
                    _orders = new OrderRepository(_context);
                }

                return _orders;
            }
        }

        public IRefreshTokenRepository _refreshTokens;
        public IRefreshTokenRepository RefreshTokens
        {
            get
            {
                if (_refreshTokens == null)
                {
                    _refreshTokens = new RefreshTokenRepository(_context);
                }

                return _refreshTokens;
            }
        }

        public IGenericRepository<Interaction> _interactions;
        public IGenericRepository<Interaction> Interactions
        {
            get
            {
                if (_interactions == null)
                {
                    _interactions = new GenericRepository<Interaction>(_context);
                }

                return _interactions;
            }
        }

        public IBillingProfileRepository _billingProfiles;
        public IBillingProfileRepository BillingProfiles
        {
            get
            {
                if (_billingProfiles == null)
                {
                    _billingProfiles = new BillingProfileRepository(_context);
                }

                return _billingProfiles;
            }
        }
    }
}
