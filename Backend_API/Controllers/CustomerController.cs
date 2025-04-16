using AutoMapper;
using Backend_API.Data.Repositories;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Helpers;
using Models.Requests;
using Models.Responses;

namespace Backend_API.Controllers
{
    [Route("Customers")]
    public class CustomerController : AuthorizationController
    {
        private readonly ICrmRepository _repository;
        private readonly ICustomerService _customerService;
        private readonly IDataValidationService _dataValidationService;
        private readonly IAssetService _assetService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public CustomerController(
            ICrmRepository crmRepository,
            ICustomerService customerService,
            IDataValidationService dataValidationService,
            IAssetService assetService,
            IOrderService orderService,
            IMapper mapper)
        {
            _repository = crmRepository;
            _dataValidationService = dataValidationService;
            _customerService = customerService;
            _assetService = assetService;
            _orderService = orderService;
            _mapper = mapper;

        }

        [HttpPost]
        public async Task<IActionResult> GetCustomers([FromBody] CustomerFilterRQ customerFilter)
        {
            if (customerFilter.IsNullOrEmpty())
            {
                return BadRequest();
            }

            var customersDTOs = new List<CustomerDTO>();

            try
            {
                var customers = await _customerService.GetCustomersAsync(customerFilter);

                if (customers.IsNullOrEmpty())
                {
                    return Problem("No customers found!");
                }

                foreach (var customer in customers)
                {
                    customersDTOs.Add(_customerService.MapCustomerToDTO(customer));
                }

                return Ok(customersDTOs);
            }
            catch (Exception ex)
            {
                //add logging
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("{customerId}")]
        public async Task<IActionResult> GetCustomer(long customerId)
        {
            var customerDTO = new CustomerDTO();

            try
            {
                var customer = await _customerService.GetCustomerDataAsync(customerId);

                if (customer.IsNullOrEmpty())
                {
                    //add logging
                    return Problem();
                }

                customerDTO = _customerService.MapCustomerToDTO(customer);
            }
            catch (Exception ex)
            {
                //add logging
                return Problem(ex.Message);
            }

            return Ok(customerDTO);
        }

        [HttpPost]
        [Route("Assets")]
        public async Task<IActionResult> GetCustomerAssets([FromBody] long id)
        {
            var customerAssets = _customerService.GetCustomerAssets(id);

            if (customerAssets.IsNullOrEmpty())
            {
                return Ok(new List<AssetDTO>());
            }

            var assetDTOs = _assetService.MapCustomerAssetsToAssetDTOs(customerAssets);

            return Ok(assetDTOs);
        }


        [HttpGet]
        [Route("Assets/{customerAssetsID}")]
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
        [Route("Create")]
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
                //add logging
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
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
                //add logging
                return StatusCode(500, ex.Message);
            }

            return Ok(new ResponseBase());
        }

        [HttpPut]
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
                //add logging
                return StatusCode(500, ex.Message);
            }

            return Ok(new ResponseBase());
        }

        [HttpGet]
        [Route("Orders/{customerID}")]
        public async Task<IActionResult> GetCustomerOrders(long customerID)
        {
            var orderDTOs = new List<OrderDTO>();

            try
            {
                var orders = _customerService.GetCustomerOrders(customerID);

                if (orders.IsNullOrEmpty())
                {
                    return Ok(new List<OrderDTO>());
                }

                foreach (var order in orders)
                {
                    var orderDTO = _orderService.MapToDTO(order);

                    orderDTOs.Add(orderDTO);
                }

                return Ok(orderDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("Interactions/{customerID}")]
        public async Task<IActionResult> GetCustomerInteractions(long customerID)
        {
            var interactionDTOs = new List<InteractionDTO>();

            try
            {
                var interactions = _customerService.GetCustomerInteractions(customerID);

                if (interactions.IsNullOrEmpty())
                {
                    return Ok(new List<InteractionDTO>());
                }

                foreach (var interaction in interactions)
                {
                    var interactionDTO = _mapper.Map<InteractionDTO>(interaction);

                    interactionDTOs.Add(interactionDTO);
                }

                return Ok(interactionDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GridFilterData")]
        public async Task<IActionResult> GetCustomerFilterBaseValues()
        {
            try
            {
                var result = _customerService.GetCustomerFilterBaseValues();

                if (result is null)
                {
                    return Problem("No data fetched!");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                //add logging
                return StatusCode(500, ex.Message);
            }
        }
    }
}
