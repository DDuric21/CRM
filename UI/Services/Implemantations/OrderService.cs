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
            var url = $"Order/{orderDTO.OrderID}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, orderDTO);

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

        public async Task CreateOrderAsync(Guid orderID, long customerID, long customerAssetID = 0)
        {
            var url = "Order";
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

            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, orderDTO);

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
            var url = "Order";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, url, orderDTO);

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
            var url = "Order";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Delete, url, custoemrAssetID);

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
