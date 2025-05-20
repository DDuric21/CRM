using Models.DTO;
using Models.Requests;

namespace UI.Services
{
    public class NewsService : INewsService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;

        public NewsService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<IEnumerable<NewsDTO>> GetNewsAsync(IEnumerable<RetrieveNewsRQ> newsFilter)
        {
            var url = "News";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, newsFilter);

            try
            {
                var response = await _communicationService.SendRequestAsync<IEnumerable<NewsDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return Array.Empty<NewsDTO>();
            }
        }
    }
}
