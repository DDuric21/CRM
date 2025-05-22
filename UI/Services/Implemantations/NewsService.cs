using Models.Requests;
using Models.Responses;

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

        public async Task<RetrieveNewsRS> GetNewsAsync(IEnumerable<RetrieveNewsRQ> newsFilter)
        {
            var url = "News";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, newsFilter);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<RetrieveNewsRS>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new RetrieveNewsRS { ErrorMessage = ex.Message };
            }
        }
    }
}
