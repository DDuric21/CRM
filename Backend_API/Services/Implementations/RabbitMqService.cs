using Backend_API.Logging;
using Backend_API.MessageCommands;
using Backend_API.Properties;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Backend_API.Services.Implementations
{
    public class RabbitMqService : IMessageBrokerService
    {
        private readonly IOptions<RabbitMqOptions> _options;

        private const string BillingQueueName = "billing_queue";
        private const string OrderStatusQueueName = "order_status_queue";
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

            var routingKey = GetRoutingKey<T>();

            await _channel.BasicPublishAsync(exchange: ExchangeName, routingKey: routingKey, mandatory: true, basicProperties: props, body: body);
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

                await _channel.QueueDeclareAsync(queue: BillingQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                await _channel.QueueDeclareAsync(queue: OrderStatusQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                await _channel.QueueBindAsync(queue: OrderStatusQueueName, exchange: ExchangeName, routingKey: OrderStatusQueueName);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += OnMessageReceivedAsync;

                await _channel.BasicConsumeAsync(queue: OrderStatusQueueName, autoAck: false, consumer: consumer);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "Failed to connect to RabbitMQ");
            }
        }

        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs eventArg)
        {
            try
            {
                var json = Encoding.UTF8.GetString(eventArg.Body.Span);
                var envelope = JsonConvert.DeserializeObject<MessageEnvelope>(json);

                switch (envelope.MessageType)
                {
                    case nameof(OrderStatusUpdateCommand):
                        var addCmd = JsonConvert.DeserializeObject<OrderStatusUpdateCommand>(envelope.Message)!;
                        await addCmd.ExecuteAsync();
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown message type {envelope.MessageType}");
                }

                await _channel.BasicAckAsync(eventArg.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "Error processing message from RabbitMQ");
                await _channel.BasicNackAsync(eventArg.DeliveryTag, multiple: false, requeue: false);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _connection?.CloseAsync();
            await _channel?.CloseAsync();
        }

        private string GetRoutingKey<T>()
        {
            return typeof(T) switch
            {
                _ when typeof(T) == typeof(UpdateBillingCommand) => BillingQueueName,
                _ when typeof(T) == typeof(OrderStatusUpdateCommand) => OrderStatusQueueName,
                _ => throw new ArgumentException($"No queue defined for command type {typeof(T).Name}")
            };
        }
    }
}
