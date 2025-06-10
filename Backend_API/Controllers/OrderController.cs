using Backend_API.Helpers;
using Backend_API.Logging;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
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
                var orderDTO = _orderService.GetOrderData(orderID);

                if (orderDTO.IsNullOrEmpty())
                {
                    return Problem("No order data found!");
                }

                return Ok(orderDTO);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRQ request)
        {
            if (request.IsNullOrEmpty()
                && request.OrderDTO.IsNullOrEmpty())
            {
                return HttpContext.BadRequest();
            }

            var result = await _orderService.CreateNewOrderAsync(request);

            if (!result.IsSuccess)
            {
                return Problem(result.ErrorMessage ?? "No order created!");
            }

            return Ok(result);
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
                var result = await _orderService.SubmitOrderDataAsync(orderDTO);

                if (!result)
                {
                    return Problem("No order created!");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
