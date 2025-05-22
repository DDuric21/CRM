using Models.Authentication.DataStructures;
using Models.DTO;
using Models.Helpers;
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
                var response = await _communicationService.SendRequestAsyncNew<ResponseBase>(request);

                return response.IsSuccess;
            }
            catch (Exception ex)
            {
                var guid = request.Headers
                    .GetValues(HttpHeaderNames.CorrelationID)?
                    .FirstOrDefault();
                Console.WriteLine($"Failed to log exception to server. GUID: {guid}");
                return false;
            }
        }

        public async Task<bool> SendErrorLogToServerAsync(Exception exception, string errorMessage = null, string stackTrace = null)
        {
            var logDetails = new LogDetails
            {
                Message = errorMessage ?? exception.Message,
                StackTrace = stackTrace ?? ExceptionHelper.FlattenExceptionMessages(exception),
                LogLevel = LogLevel.Error.ToString(),
                Url = exception.TargetSite?.DeclaringType?.FullName ?? "Unknown",
            };

            return await SendErrorLogToServerAsync(logDetails);
        }
    }
}
