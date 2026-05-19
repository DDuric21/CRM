using Backend_API.Data.Models;

namespace Backend_API.Services
{
    public interface IQueueActionHandler
    {
        /// <summary>
        /// The Type string this handler supports, e.g. "Logging" or "Email".
        /// </summary>
        string ActionType { get; }

        /// <summary>
        /// The maximum number of attempts to retry.
        /// </summary>
        int RetryAttempts { get; }

        /// <summary>
        /// Run the work for the given queueAction.
        /// </summary>
        Task HandleAsync(QueueAction queueAction, CancellationToken token = default);
    }
}
