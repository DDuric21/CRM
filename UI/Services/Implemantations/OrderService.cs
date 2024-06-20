using Models.DTO;
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

        public async Task AddAsset(OrderDTO orderDTO)
        {
            var url = string.Format("https://localhost:7076/Order");
            var request = _communicationService.CreateRequest(HttpMethod.Post, url, orderDTO);

            try
            {
                var response = await _communicationService.SendRequestAsync<ResponseBase>(request);                
            }
            catch (Exception ex)
            {
                // loging
                _modalService.ShowErrorMessage(ex.Message);
            }
        }

        public async Task UpdateAsset(OrderDTO orderDTO)
        {
            
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
                // loging
                _modalService.ShowErrorMessage(ex.Message);
                return 0;
            }
        }
    }
}
