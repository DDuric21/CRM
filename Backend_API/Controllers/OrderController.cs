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
                var orderData = _orderService.MapToOrderData(orderDTO);
                await _orderService.CreateOrder(orderData);

                if (orderData.Id > 0)
                {
                    return Ok(new ResponseBase());
                }

                return Problem("No order created!");
            }
            catch (Exception ex)
            {
                //add loging
                return Problem(ex.Message);
            }
        }

    }
}
