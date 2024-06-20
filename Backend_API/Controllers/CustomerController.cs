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
        private readonly IAssetService _assetService;

        public CustomerController(
            ICrmRepository crmRepository,
            ICustomerService customerService,
            IDataValidationService dataValidationService,
            IAssetService assetService)
        {
            _repository = crmRepository;
            _dataValidationService = dataValidationService;
            _customerService = customerService;
            _assetService = assetService;
        }

        [HttpGet]
        [Route("/Customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customersDTOs = new List<CustomerDTO>();

            try
            {
                var customers = await _repository.Customers.GetAllCustomersAsync();

                foreach (var customer in customers)
                {
                    customersDTOs.Add(_customerService.MapCustomerToDTO(customer));
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return Ok(customersDTOs);
        }

        [HttpGet]
        [Route("/Customers/{id}")]
        public async Task<IActionResult> GetCustomer(long id)
        {
            var customerDTO = new CustomerDTO();

            try
            {
                var customer = _customerService.GetCustomerData(id);

                if (customer.IsNullOrEmpty())
                {
                    //add loging
                    return Problem();
                }

                customerDTO = _customerService.MapCustomerToDTO(customer);
            }
            catch (Exception ex)
            {
                //add loging
                return Problem(ex.Message);
            }

            return Ok(customerDTO);
        }

        [HttpPost]
        [Route("/Customers/Assets")]
        public async Task<IActionResult> GetCustomerAssets([FromBody] long id)
        {
            var assets = _customerService.GetCustomerAssets(id);

            if (assets.IsNullOrEmpty())
            {
                return Ok(new List<AssetDTO>());
            }

            var assetDTOs = _assetService.MapAssetsToDTOs(assets);

            return Ok(assetDTOs);
        }


        [HttpGet]
        [Route("/Customers/Assets/{customerAssetsID}")]
        public async Task<IActionResult> GetCustomerAssetData(long customerAssetsID)
        {
            if (customerAssetsID <= 0)
            {
                return BadRequest("Incorrect ID");
            }

            var asset = _customerService.GetCustomerAssetData(customerAssetsID);

            if (asset.IsNullOrEmpty())
            {
                return Ok(new AssetDTO());
            }

            var assetDTO = _assetService.MapAssetToDTO(asset, customerAssetsID);

            return Ok(assetDTO);
        }

        [HttpPost]
        [Route("/Customers")]
        public async Task<IActionResult> InsertCustomer([FromBody] CustomerDTO customerDTO)
        {
            if (!_dataValidationService.ValidateCustomerDTO(customerDTO))
            {
                return BadRequest();
            }

            var customer = _customerService.MapDtoToCustomer(customerDTO);

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
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDTO customerDTO)
        {
            if (!_dataValidationService.ValidateCustomerDTO(customerDTO))
            {
                return BadRequest();
            }

            var customer = _customerService.MapDtoToCustomer(customerDTO);

            try
            {
                var result = await _repository.Customers.UpdateCustomerAsync(customer);

                if (result == 0)
                {
                    return Problem("No elemets modified!");
                }
            }
            catch (Exception ex)
            {
                //add loging
                return StatusCode(500, ex.Message);
            }

            return Ok(new ResponseBase());
        }
    }
}
