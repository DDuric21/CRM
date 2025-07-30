using Backend_API.Logging;
using Backend_API.MessageCommands;
using Backend_API.Properties;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Backend_API.Services.Implementations
{
    public class RabbitMqService : IMessageBrokerService
    {
        private readonly IOptions<RabbitMqOptions> _options;

        private const string QueueName = "billing_queue";
        private const string ExchangeName = "commands";

        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqService(IOptions<RabbitMqOptions> options)
        {
            _options = options;
        }

        public async Task PublishAsync<T>(T cmd)
        {
            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

            var envelope = new MessageEnvelope
            {
                MessageType = typeof(T).Name,
                Message = JsonConvert.SerializeObject(cmd)
            };

            var jsonBody = JsonConvert.SerializeObject(envelope);
            var body = Encoding.UTF8.GetBytes(jsonBody);

            await _channel.BasicPublishAsync(exchange: ExchangeName, routingKey: QueueName, mandatory: true, basicProperties: props, body: body);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var config = _options.Value;

            var factory = new ConnectionFactory
            {
                HostName = config.HostName,
                Port = config.Port,
                UserName = config.UserName,
                Password = config.Password,
                VirtualHost = config.VirtualHost
            };

            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
                await _channel.QueueDeclareAsync(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "Failed to connect to RabbitMQ");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _connection?.CloseAsync();
            await _channel?.CloseAsync();
        }
    }
}
