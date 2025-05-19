using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public class LoggingService : ILoggingService
    {
        private const string ControllerEndpoint = "Logging";
        private const string ErrorAction = "Error";
        private readonly ICommunicationService _communicationService;

        public LoggingService(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        public async Task<bool> SendErrorLogToServerAsync(LogDetails logDetails)
        {
            var url = $"{ControllerEndpoint}/{ErrorAction}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, logDetails, false);
            try
            {
                var response = await _communicationService.SendRequestAsync<ResponseBase>(request);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
