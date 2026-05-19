using Models.Requests;
using Models.Responses;

namespace UI.Services
{
    public class NewsService : INewsService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;
        private const string ApiUrl = "News";

        public NewsService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<RetrieveNewsRS> GetNewsAsync(IEnumerable<RetrieveNewsRQ> newsFilter)
        {
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, ApiUrl, newsFilter);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<RetrieveNewsRS>(request);

                if (response == null 
                    || !response.IsSuccess)
                {
                    return new RetrieveNewsRS(false, errorMessage: "No news items found.");
                }

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
