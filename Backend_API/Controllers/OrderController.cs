using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.HelperMethods;
using Models.Responses;

namespace Backend_API.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(
            IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        [Route("/Order")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO orderDTO)
        {
            if (orderDTO.IsNullOrEmpty())
            {
                return BadRequest();
            }

            try
            {
                var order = _orderService.MapDtoToOrder(orderDTO);
                await _orderService.CreateOrderAsync(order);

                return Ok(new ResponseBase());
            }
            catch (Exception ex)
            {
                //add loging
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("/Order/{orderID}")]
        public async Task<IActionResult> SubmitOrder([FromBody] OrderDTO orderDTO)
        {
            if (orderDTO.IsNullOrEmpty())
            {
                return BadRequest();
            }

            try
            {
                var orderAsset = _orderService.MapToCustomerAsset(orderDTO);
                await _orderService.SubmitOrderAsync(orderAsset);

                if (orderAsset.Id > 0)
                {
                    var orderAssetOptions = _orderService.MapToCustomerAssetOptions(orderDTO, orderAsset);
                    await _orderService.CreateOrderAssetOptionsAsync(orderAssetOptions); // nemam provjere da li su uneseni

                    return Ok(new ResponseBase());
                }

                return Problem("No order created!");
            }
            catch (Exception ex)
            {
                //add loging
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("/Order")]
        public async Task<IActionResult> DeleteCustomerAsset([FromBody] long customerAssetID)
        {
            if (customerAssetID <= 0)
            {
                return BadRequest();
            }

            try
            {
                var result = await _orderService.DeleteCustomerAssetAsync(customerAssetID);

                return Ok(result);
            }
            catch (Exception ex)
            {
                //add loging
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("/Order")]
        public async Task<IActionResult> UpdateCustomerAsset([FromBody] OrderDTO orderDTO)
        {
            if (orderDTO.IsNullOrEmpty()
                || orderDTO.AssetDTO.IsNullOrEmpty()
                || orderDTO.AssetDTO.CustomerAssetID <= 0)
            {
                return BadRequest();
            }

            var customerAsset = _orderService.MapToCustomerAssetData(orderDTO);

            try
            {
                var result = await _orderService.UpdateCustomerAssetAsync(customerAsset);

                return Ok(result);
            }
            catch (Exception ex)
            {
                //add loging
                return StatusCode(500, ex.Message);
            }
        }
    }
}
