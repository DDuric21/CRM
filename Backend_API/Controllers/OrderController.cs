using Backend_API.Helpers;
using Backend_API.Logging;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Helpers;
using Models.Requests;
using Models.Responses;
using Resources.Translations.API;

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
                return HttpContext.BadRequest();
            }

            var orderDTO = await _orderService.GetOrderDataAsync(orderID);

            if (orderDTO.IsNullOrEmpty())
            {
                return Problem(APITranslations.OrderNotFound);
            }

            return Ok(new GetOrderDataRS { Order = orderDTO });
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

        [HttpPut]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRQ cancelOrderRQ)
        {
            if (cancelOrderRQ.IsNullOrEmpty())
            {
                return HttpContext.BadRequest();
            }

            var result = await _orderService.CancelOrderAsync(cancelOrderRQ);

            if (!result.IsSuccess)
            {
                return Problem(result.ErrorMessage ?? APITranslations.OrderNotCanceled);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GridFilterData")]
        public async Task<IActionResult> GetOrderFilterBaseValues()
        {
            var filterData = await _orderService.GetOrderFilterBaseValuesAsync();

            if (filterData.IsNullOrEmpty())
            {
                return Problem(APITranslations.NoGridFilterDataFound);
            }

            return Ok(filterData);
        }


        [HttpPost]
        [Route("GetOrders")]
        public async Task<IActionResult> GetOrders([FromBody] OrderFilterRQ orderFilter)
        {
            var orders = await _orderService.GetOrdersAsync(orderFilter);

            return Ok(orders);
        }
    }
}
