using Models.DTO;
using Models.Helpers;
using Models.Requests;
using Models.Responses;
using Resources.Translations;
using UI.Helpers;

namespace UI.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;
        private const string ApiUrl = "Order";

        public OrderService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<ResponseBase> SubmitOrderAsync(OrderDTO orderDTO)
        {
            var url = $"{ApiUrl}/{orderDTO.OrderID}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, orderDTO);

            try
            {
                var response = await _communicationService.SendRequestAsync<ResponseBase>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ResponseBase(false, ex.Message);
            }
        }

        public async Task<OrderDTO> GetOrderDataAsync(Guid id)
        {
            var url = $"{ApiUrl}/{id}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<GetOrderDataRS>(request);

                return response.Order;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new OrderDTO();
            }
        }

        public async Task<ResponseBase> CreateOrderAsync(OrderDTO orderDTO, bool withOptions = false)
        {
            var createOrderRQ = new CreateOrderRQ
            {
                OrderDTO = orderDTO,
                WithOptions = withOptions
            };

            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, ApiUrl, createOrderRQ);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<ResponseBase>(request);
                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ResponseBase(false, ex.Message);
            }
        }

        public async Task<ResponseBase> CancelOrderAsync(Guid id)
        {
            var cancelOrderRQ = new CancelOrderRQ { OrderId = id };

            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, ApiUrl, cancelOrderRQ);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<ResponseBase>(request);
                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ResponseBase(false, ex.Message);
            }
        }

        public async Task<ActionResult<OrderGridFilterDataRS>> GetOrderFilterBaseValuesAsync()
        {
            var url = $"{ApiUrl}/GridFilterData";

            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<OrderGridFilterDataRS>(request);

                if (response.IsNullOrEmpty())
                {
                    return new ActionResult<OrderGridFilterDataRS>(Translation.NoFilterDataFound);
                }

                return new ActionResult<OrderGridFilterDataRS>(response);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<OrderGridFilterDataRS>(ex.Message);
            }
        }

        public async Task<ActionResult<GetOrdersRS>> GetOrdersAsync(OrderFilter orderFilter)
        {
            var url = $"{ApiUrl}/GetOrders";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, orderFilter);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<GetOrdersRS>(request);

                return new ActionResult<GetOrdersRS>(response);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<GetOrdersRS>(ex.Message);
            }
        }
    }
}
