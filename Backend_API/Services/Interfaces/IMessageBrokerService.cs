using Backend_API.MessageCommands;

namespace Backend_API.Services
{
    public interface IMessageBrokerService : IHostedService
    {
        /// <summary>
        /// Publishes a command to the message broker.
        /// </summary>
        /// <typeparam name="T">The type of the command, which must inherit from BaseCommand.</typeparam>
        /// <param name="cmd">The command to publish.</param>
        Task PublishAsync<T>(T cmd);
    }
}
