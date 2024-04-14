using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.HelperMethods;
using Models.Responses;

namespace Backend_API.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICrmRepository _repository;
        private readonly ICustomerService _customerService;
        private readonly IDataValidationService _dataValidationService;

        public CustomerController(
            ICrmRepository crmRepository,
            ICustomerService customerService,
            IDataValidationService dataValidationService)
        {
            _repository = crmRepository;
            _dataValidationService = dataValidationService;
            _customerService = customerService;
        }

        [HttpGet]
        [Route("/Customers")]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _repository.Customers.GetAllAsync();

            return Ok(customers);
        }

        [HttpGet]
        [Route("/Customers/{id}")]
        public async Task<CustomerDTO> GetById(long id)
        {
            var customer = await _repository.Customers.GetByIdAsync(id);

            if (customer.IsNullOrEmpty())
            {
                //add loging
                return new CustomerDTO();
            }

            var customerDTO = _customerService.MapCustomerToDTO(customer);

            return customerDTO;
        }

        [HttpPost]
        [Route("/Customers")]
        public async Task<IActionResult> InsertCustomer([FromBody] CustomerDTO customerDTO)
        {
            if (!_dataValidationService.ValidateCustomerDTO(customerDTO))
            {
                return BadRequest();
            }

            var customer = new Customer();
            customer.Name = customerDTO.Name;
            customer.Birthday = customerDTO.Birthday;
            if (!customerDTO.Address.IsNullOrEmpty())
            {
                customer.Address = new Address();
                customer.Address.FullAddress = customerDTO.Address?.FullAddress;
            }

            try
            {
                await _repository.Customers.InsertAsync(customer);
                return Ok(customer.Id);
            }
            catch (Exception ex)
            {
                //add loging
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("/Customers/{id}")]
        public async Task<IActionResult> DeleteCustomer(long id)
        {
            try
            {
                var result = await _repository.Customers.DeleteByIdAsync(id);
                if (result == 0)
                {
                    return Problem("No enities deleted!");
                }
            }
            catch (Exception ex)
            {
                //add loging
                return StatusCode(500, ex.Message);
            }

            return Ok(new ResponseBase());
        }

        [HttpPut]
        [Route("/Customers")]
        public int UpdateCustomer(Customer customer)
        {
            try
            {
                return _repository.Customers.UpdateAsync(customer).Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return 0;
        }
    }
}
