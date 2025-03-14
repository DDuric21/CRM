using Models.DTO;
using Models.Requests;
using Models.Responses;

namespace UI.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ICrmModalService _modalService;
        private const int ExceptionResponse = -1;

        public OrderService(
            ICommunicationService communicationService,
            ICrmModalService modalService)
        {
            _communicationService = communicationService;
            _modalService = modalService;
        }

        public async Task<int> SubmitOrderAsync(OrderDTO orderDTO)
        {
            var url = string.Format("https://localhost:7076/Order/{0}", orderDTO.OrderID);
            var request = _communicationService.CreateRequest(HttpMethod.Post, url, orderDTO);

            try
            {
                var response = await _communicationService.SendRequestAsync<int>(request);

                return response;
            }
            catch (Exception ex)
            {
                // logging
                _modalService.ShowErrorMessage(ex.Message);
                return ExceptionResponse;
            }
        }

        public async Task<OrderDTO> GetOrderDataAsync(Guid id)
        {
            var url = string.Format("https://localhost:7076/Order/{0}", id);
            var request = _communicationService.CreateRequest(HttpMethod.Get, url);

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

            var url = string.Format("https://localhost:7076/Order");

            var request = _communicationService.CreateRequest(HttpMethod.Post, url, createOrderRQ);

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

        public async Task CreateOrderAsync(Guid orderID, long customerID, long customerAssetID = 0)
        {
            var url = string.Format("https://localhost:7076/Order");
            var orderDTO = new OrderDTO
            {
                OrderID = orderID,
                CustomerDTO = new CustomerDTO
                {
                    Id = customerID,
                },
                AssetDTO = new AssetDTO
                {
                    CustomerAssetID = customerAssetID
                }
            };

            var request = _communicationService.CreateRequest(HttpMethod.Post, url, orderDTO);

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

        public async Task<int> UpdateAsset(OrderDTO orderDTO)
        {
            var url = string.Format("https://localhost:7076/Order");
            var request = _communicationService.CreateRequest(HttpMethod.Put, url, orderDTO);

            try
            {
                var response = await _communicationService.SendRequestAsync<int>(request);

                return response;
            }
            catch (Exception ex)
            {
                // logging
                _modalService.ShowErrorMessage(ex.Message);
                return 0;
            }
        }

        public async Task<int> DeleteAsset(long custoemrAssetID)
        {
            var url = string.Format("https://localhost:7076/Order");
            var request = _communicationService.CreateRequest(HttpMethod.Delete, url, custoemrAssetID);

            try
            {
                var response = await _communicationService.SendRequestAsync<int>(request);

                return response;
            }
            catch (Exception ex)
            {
                // logging
                _modalService.ShowErrorMessage(ex.Message);
                return 0;
            }
        }
    }
}
