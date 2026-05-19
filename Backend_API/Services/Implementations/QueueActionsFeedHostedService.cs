using Backend_API.Data.Repositories;
using Backend_API.Logging;

namespace Backend_API.Services
{
    public class QueueActionsFeedHostedService : BackgroundService
    {
        private readonly IServiceProvider _svcProvider;
        private readonly QueueActionDispatcher _dispatcher;

        public QueueActionsFeedHostedService(IServiceProvider svcProvider, QueueActionDispatcher dispatcher)
        {
            _svcProvider = svcProvider;
            _dispatcher = dispatcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DynamicLogger.LogInfo("QueueActions Feeding started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _svcProvider.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IQueueActionRepository>();

                    var action = await repository.DequeueAsync(stoppingToken);
                    if (action is null)
                    {
                        await Task.Delay(2_000, stoppingToken);
                        continue;
                    }

                    await _dispatcher.ExecuteAsync(action, stoppingToken);
                    await repository.UpdateAsync(action);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    DynamicLogger.LogException(ex, "Unexpected error in feed loop");
                    await Task.Delay(5_000, stoppingToken);
                }
            }

            DynamicLogger.LogInfo("QueueActions Feeding stopping.");
        }
    }
}
