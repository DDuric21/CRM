using Models.DTO;
using Models.Requests;
using Models.Responses;

namespace UI.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;

        public OrderService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<bool> SubmitOrderAsync(OrderDTO orderDTO)
        {
            var url = $"Order/{orderDTO.OrderID}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, orderDTO);

            try
            {
                var response = await _communicationService.SendRequestAsync<bool>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return false;
            }
        }

        public async Task<OrderDTO> GetOrderDataAsync(Guid id)
        {
            var url = $"Order/{id}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<OrderDTO>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new OrderDTO();
            }
        }

        public async Task<bool> CreateOrderAsync(OrderDTO orderDTO, bool withOptions = false)
        {
            var createOrderRQ = new CreateOrderRQ
            {
                OrderDTO = orderDTO,
                WithOptions = withOptions
            };

            var url = "Order";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, createOrderRQ);

            try
            {
                var response = await _communicationService.SendRequestAsync<ResponseBase>(request);
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return false;
            }
        }
    }
}
