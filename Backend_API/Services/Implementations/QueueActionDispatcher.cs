using Backend_API.Data.Models;
using Backend_API.Logging;

namespace Backend_API.Services
{
    public class QueueActionDispatcher
    {
        private readonly IServiceProvider _svcProvider;

        public QueueActionDispatcher(IServiceProvider svcProvider)
        {
            _svcProvider = svcProvider;
        }

        public async Task ExecuteAsync(QueueAction queueAction, CancellationToken token)
        {
            using var scope = _svcProvider.CreateScope();
            var handlers = scope.ServiceProvider.GetServices<IQueueActionHandler>();

            var handler = handlers.Single(h => h.ActionType == queueAction.Type);
            if (handler is null)
            {
                DynamicLogger.LogError($"No handler registered for job type {queueAction.Type}");
                throw new InvalidOperationException($"Unknown job type: {queueAction.Type}");
            }

            await handler.HandleAsync(queueAction, token);
        }
    }
}
