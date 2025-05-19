using Models.DTO;

namespace UI.Services
{
    public interface ILoggingService
    {
        Task<bool> SendErrorLogToServerAsync(LogDetails logDetails);
    }
}
