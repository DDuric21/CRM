using Backend_API.Logging;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Enums;
using Models.Helpers;
using Models.Requests;
using Models.Responses;

namespace Backend_API.Controllers
{
    [Route("/Order")]
    public class OrderController : AuthorizationController
    {
        private readonly IOrderService _orderService;

        public OrderController(
            IOrderService orderService)
        {
            _orderService = orderService;
        }


        [HttpGet]
        [Route("{orderID:Guid}")]
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
                DynamicLogger.LogException(ex, nameof(GetOrderData), ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
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
                DynamicLogger.LogException(ex, nameof(CreateOrder), ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("{orderID}")]
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
                DynamicLogger.LogException(ex, nameof(SubmitOrder), ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
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
                DynamicLogger.LogException(ex, nameof(DeactivateCustomerAsset), ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
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
                DynamicLogger.LogException(ex, nameof(UpdateCustomerAsset), ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
