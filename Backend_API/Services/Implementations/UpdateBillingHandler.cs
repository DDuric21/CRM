using Backend_API.Data.Models;
using Backend_API.Logging;
using Backend_API.MessageCommands;
using Models.Enums;
using Newtonsoft.Json;

namespace Backend_API.Services.Implementations
{
    public class UpdateBillingHandler : IQueueActionHandler
    {
        private readonly IMessageBrokerService _messageBroker;
        public UpdateBillingHandler(IMessageBrokerService messageBroker)
        {
            _messageBroker = messageBroker;
        }

        public string ActionType => nameof(UpdateBillingCommand);

        public int RetryAttempts => 3;

        public async Task HandleAsync(QueueAction queueAction, CancellationToken token = default)
        {
            var payload = JsonConvert.DeserializeObject<UpdateBillingCommand>(queueAction.Payload)!;
            try
            {
                await _messageBroker.PublishAsync(payload);
                queueAction.StatusId = (int)QueueActionStatus.Completed;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, $"Executing queueActionID: {queueAction.Id} failed.");

                queueAction.StatusId = queueAction.Attempts >= RetryAttempts 
                    ? (int)QueueActionStatus.Failed
                    : (int)QueueActionStatus.Pending;
                queueAction.LastError = ex.Message;
                queueAction.LastAttemptedAt = DateTime.UtcNow;
                queueAction.Attempts++;
            }
        }
    }
}
