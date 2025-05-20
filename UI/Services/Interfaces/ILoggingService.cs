using Models.DTO;

namespace UI.Services
{
    public interface ILoggingService
    {
        Task<bool> SendErrorLogToServerAsync(LogDetails logDetails);

        Task<bool> SendErrorLogToServerAsync(Exception exception, string errorMessage = null, string stackTrace = null);
    }
}
