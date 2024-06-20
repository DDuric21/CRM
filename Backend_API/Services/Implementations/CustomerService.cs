using AutoMapper;
using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Models.DTO;
using Models.HelperMethods;

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
            var customer = _repository.Customers
                .Filter(
                    filter: x => x.Id == id, 
                    include: x => x.Addresses)
                .FirstOrDefault();

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

        public Dictionary<long, Asset> GetCustomerAssets(long id)
        {
            var assets = _repository.CustomerAssets
                .With(x => x.Asset)
                .Where(x => x.CustomerID == id)
                .ToDictionary(x => x.Id, x => x.Asset);

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
    }
}
