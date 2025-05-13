using Models.DTO;
using Models.Requests;
using Models.Responses;

namespace UI.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ICrmModalService _modalService;

        public OrderService(
            ICommunicationService communicationService,
            ICrmModalService modalService)
        {
            _communicationService = communicationService;
            _modalService = modalService;
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
                // logging
                _modalService.ShowErrorMessage(ex.Message);
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
                // logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }

        public async Task CreateOrderAsync(OrderDTO orderDTO, bool withOptions = false)
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
            }
            catch (Exception ex)
            {
                // logging
                _modalService.ShowErrorMessage(ex.Message);
            }
        }
    }
}
