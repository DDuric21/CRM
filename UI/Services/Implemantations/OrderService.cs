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
    }
}
