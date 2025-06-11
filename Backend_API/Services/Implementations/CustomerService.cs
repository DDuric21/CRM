using AutoMapper;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Models.DTO;
using Models.Enums;
using Models.Helpers;
using Models.Requests;
using Models.Responses;

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

        public async Task<Customer> GetCustomerDataAsync(long customerId)
        {
            var customerData = await _repository.Customers.GetAllCustomerRelatedDataAsync(customerId);

            if (customerData.IsNullOrEmpty())
            {
                return new Customer();
            }

            return customerData;
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

        public async Task<CustomerAssets> GetCustomerAssetDataAsync(long customerAssetsID)
        {
            var retrievedData = await _repository.CustomerAssets
                .Where(x => x.Id == customerAssetsID)
                .Include(x => x.Asset)
                .Include(x => x.BillingProfile)
                .Include(x => x.CustomerAssetOptions)
                .ThenInclude(y => y.Option)
                .FirstOrDefaultAsync();

            return retrievedData;
        }

        public async Task<List<Order>> GetCustomerOrdersAsync(long customerID)
        {
            var retrievedData = await _repository.Orders
                .Where(x => x.CustomerID == customerID)
                .Include(x => x.CreatedByUser)
                .ToListAsync();

            return retrievedData;
        }

        public List<Interaction> GetCustomerInteractions(long customerID)
        {
            var interactions = _repository.Interactions
                .Where(x => x.CustomerID == customerID)
                .ToList();

            return interactions;
        }

        public CustomerGridFilterDataRS GetCustomerFilterBaseValues()
        {
            var customerTypes = Enum.GetValues(typeof(CustomerType))
                .Cast<CustomerType>()
                .Where(x => x == CustomerType.Residential)
                .ToList();

            var customerStatuses = Enum.GetValues(typeof(ItemState))
                .Cast<ItemState>()
                .ToList();

            var customerFilterBaseValues = new CustomerGridFilterDataRS
            {
                CustomerTypes = customerTypes,
                CustomerStatuses = customerStatuses
            };

            return customerFilterBaseValues;
        }

        public async Task<List<Customer>> GetCustomersAsync(CustomerFilterRQ customerFilter)
        {
            var customers = _repository.Customers
                .With(x => x.Addresses);

            customers = ApplyCustomerFilters(customers, customerFilter);

            var filteredCustomers = await customers.ToListAsync();

            return filteredCustomers;
        }

        private IQueryable<Customer> ApplyCustomerFilters(IQueryable<Customer> customers, CustomerFilterRQ customerFilter)
        {
            if (!customerFilter.CustomerTypes.IsNullOrEmpty())
            {
                customers = customers.Where(x => customerFilter.CustomerTypes.Contains((CustomerType)x.TypeID));
            }

            if (!customerFilter.CustomerStatuses.IsNullOrEmpty())
            {
                customers = customers.Where(x => customerFilter.CustomerStatuses.Contains((ItemState)x.CustomerStatusID));
            }

            if (customerFilter.PersonalID != null)
            {
                customers = customers.Where(x => x.PersonalID == customerFilter.PersonalID);
            }

            if (customerFilter.FirstName != null)
            {
                customers = customers.Where(x => x.FirstName.StartsWith(customerFilter.FirstName));
            }

            if (customerFilter.LastName != null)
            {
                customers = customers.Where(x => x.LastName.StartsWith(customerFilter.LastName));
            }

            if (customerFilter.BirthdayDateStart.HasValue)
            {
                customers = customers.Where(x => x.Birthday >= customerFilter.BirthdayDateStart.Value);
            }

            if (customerFilter.BirthdayDateEnd.HasValue)
            {
                customers = customers.Where(x => x.Birthday <= customerFilter.BirthdayDateEnd.Value);
            }

            return customers;
        }
    }
}
