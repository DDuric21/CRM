using Models.DTO;
using Models.Requests;

namespace UI.Services
{
    public class NewsService : INewsService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ICrmModalService _modalService;

        public NewsService(
            ICommunicationService communicationService,
            ICrmModalService modalService)
        {
            _communicationService = communicationService;
            _modalService = modalService;
        }

        public async Task<IEnumerable<NewsDTO>> GetNewsAsync(IEnumerable<RetrieveNewsRQ> newsFilter)
        {
            var url = string.Format("https://localhost:7076/News");
            var request = _communicationService.CreateRequest(HttpMethod.Post, url, newsFilter);

            try
            {
                var response = await _communicationService.SendRequestAsync<IEnumerable<NewsDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                // loging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }
    }
}
