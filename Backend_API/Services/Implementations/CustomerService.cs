using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Models.DTO;
using Models.HelperMethods;

namespace Backend_API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICrmRepository _repository;
        private readonly IAddressService _addressService;

        public CustomerService(
            IAddressService addressService,
            ICrmRepository repository)
        {
            _addressService = addressService;
            _repository = repository;
        }

        public CustomerDTO MapCustomerToDTO(Customer customer)
        {
            if (customer.IsNullOrEmpty())
            {
                return new CustomerDTO();
            }

            var customerDTO = new CustomerDTO
            {
                Name = customer.Name,
                Address = _addressService.MapAddressToDTO(customer.Address),
                Birthday = customer.Birthday,
            };

            return customerDTO;
        }
    }
}
