using AutoMapper;
using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Models.DTO;
using Models.Helpers;

namespace Backend_API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICrmRepository _repository;
        private readonly IMapper _mapper;

        public CustomerService(
            ICrmRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public Customer GetCustomerData(long id)
        {
            var customerData = _repository.Customers
                .Filter(
                    filter: x => x.Id == id, 
                    include: x => x.Addresses)
                .Select(x => new 
                {
                    Customer = x,
                    BillingProfiles = x.BillingProfiles
                })
                .FirstOrDefault();

            if (customerData.IsNullOrEmpty())
            {
                return new Customer();
            }

            var customer = customerData.Customer;
            customer.BillingProfiles = customerData.BillingProfiles;

            return customer;
        }

        public CustomerDTO MapCustomerToDTO(Customer customer)
        {
            if (customer.IsNullOrEmpty())
            {
                return new CustomerDTO();
            }

            var customerDTO = _mapper.Map<CustomerDTO>(customer);
            customerDTO.Addresses = new List<AddressDTO>();

            foreach (var address in customer.Addresses ?? Enumerable.Empty<Address>())
            {
                customerDTO.Addresses.Add(_mapper.Map<AddressDTO>(address));
            }

            customerDTO.BillingProfiles = new List<BillingProfileDTO>();

            foreach (var billingProfile in customer.BillingProfiles ?? Enumerable.Empty<BillingProfile>())
            {
                var billingProfileDTO = _mapper.Map<BillingProfileDTO>(billingProfile);
                billingProfileDTO.BillingAddress = customerDTO.Addresses
                    .FirstOrDefault(x => x.Id == billingProfile.AddressID);

                customerDTO.BillingProfiles.Add(billingProfileDTO);
            }

            return customerDTO;
        }

        public Customer MapDtoToCustomer(CustomerDTO customerDTO)
        {
            if (customerDTO.IsNullOrEmpty())
            {
                return new Customer();
            }

            var customer = _mapper.Map<Customer>(customerDTO);
            customer.Addresses = new List<Address>();

            foreach (var addressDTO in customerDTO.Addresses)
            {
                customer.Addresses.Add(_mapper.Map<Address>(addressDTO));
            }

            return customer;
        }

        public IEnumerable<CustomerAssets> GetCustomerAssets(long id)
        {
            var assets = _repository.CustomerAssets
                .With(x => x.Asset)
                .Where(x => x.CustomerID == id)
                .ToList();

            return assets;
        }

        public Asset GetCustomerAssetData(long customerAssetsID)
        {
            var retrievedData = _repository.CustomerAssets
                .Where(x => x.Id == customerAssetsID)
                .Select(y => new
                {
                    Asset = y.Asset,
                    Options = y.CustomerAssetOptions.Select(z => z.Option).ToList()
                })
                .FirstOrDefault();

            var asset = new Asset();

            if (!retrievedData.IsNullOrEmpty())
            {
                asset = retrievedData.Asset;
                asset.Options = retrievedData.Options;
            }

            return asset;
        }

        public List<Order> GetCustomerOrders(long customerID)
        {
            var retrievedData = _repository.Orders
                .Where(x => x.CustomerID == customerID)
                .ToList();

            return retrievedData;
        }

        public List<Interaction> GetCustomerInteractions(long customerID)
        {
            var interactions = _repository.Interactions
                .Where(x => x.CustomerID == customerID)
                .ToList();

            return interactions;
        }
    }
}
