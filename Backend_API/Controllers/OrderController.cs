using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Enums;
using Models.Helpers;
using Models.Requests;
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


        [HttpGet]
        [Route("/Order/{orderID:Guid}")]
        public async Task<IActionResult> GetOrderData(Guid orderID)
        {
            if (orderID == Guid.Empty)
            {
                return BadRequest();
            }

            try
            {
                var order = _orderService.GetOrderData(orderID);
                var orderDTO = _orderService.MapToDTO(order);

                if (order.IsNullOrEmpty())
                {
                    return Problem("No order data found!");
                }

                return Ok(orderDTO);
            }
            catch (Exception ex)
            {
                //add loging
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("/Order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRQ request)
        {
            if (request.IsNullOrEmpty()
                && request.OrderDTO.IsNullOrEmpty())
            {
                return BadRequest();
            }

            var orderDTO = request.OrderDTO;

            try
            {
                orderDTO.OrderStatus = OrderStatus.Open;
                var order = _orderService.MapDtoToOrder(orderDTO, request.WithOptions);
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
                orderDTO.Action = _orderService.DefineOrderAction(orderDTO.Action);
                var order = _orderService.MapDtoToOrder(orderDTO);
                var result = await _orderService.SubmitOrderAsync(order);

                if (result > 0)
                {
                    return Ok(result);
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
        public async Task<IActionResult> DeactivateCustomerAsset([FromBody] OrderDTO orderDTO)
        {
            if (orderDTO.IsNullOrEmpty())
            {
                return BadRequest();
            }

            try
            {
                var order = _orderService.MapDtoToOrder(orderDTO);
                var result = await _orderService.DeactivateCustomerAssetAsync(order);

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

            var customerAsset = _orderService.MapToCustomerAssetBasicData(orderDTO);

            try
            {
                var result = await _orderService.UpdateCustomerAssetAsync(customerAsset, orderDTO.OrderID);

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
