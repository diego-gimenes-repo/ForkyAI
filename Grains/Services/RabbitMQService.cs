using System;
using System.Text;
using Grains.Configuration;     // If you have a config class named RabbitMQOptions
using Grains.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// These two namespaces are critical for RabbitMQ .NET
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Grains.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly ILogger<RabbitMQService> _logger;
        private readonly RabbitMQOptions _options;

        // Mark them nullable if you do not initialize them in the constructor.
        private IConnection? _connection;
        private IModel? _channel;

        private bool _disposed;

        // Checks if both _connection and _channel are connected/open
        public bool IsConnected =>
            _connection is { IsOpen: true } &&
            _channel is { IsOpen: true };

        public RabbitMQService(
            ILogger<RabbitMQService> logger,
            IOptions<RabbitMQOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            InitializeConnection();
        }

        private void InitializeConnection()
        {
            try
            {
                // Ensure you’re referencing RabbitMQ.Client.ConnectionFactory
                var factory = new ConnectionFactory
                {
                    HostName = _options.HostName,
                    UserName = _options.UserName,
                    Password = _options.Password,
                    Port = _options.Port,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };

                // These methods come from RabbitMQ.Client
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: _options.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // The IConnection interface has a ConnectionShutdown event
                _connection.ConnectionShutdown += OnConnectionShutdown;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
                throw;
            }
        }

        public void PublishMessage(string message, string model)
        {
            if (!IsConnected)
            {
                _logger.LogWarning("Cannot publish message - RabbitMQ connection is not available");
                return;
            }

            try
            {
                var combinedMessage = $"{model ?? "[no-model]"}: {message}";
                var body = Encoding.UTF8.GetBytes(combinedMessage);

                // CreateBasicProperties() and BasicPublish() come from IModel
                var properties = _channel!.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: _options.QueueName,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Published message: {Message}", combinedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message");
                throw;
            }
        }

        public void SubscribeToMessages(Action<string> messageHandler)
        {
            if (!IsConnected)
            {
                _logger.LogWarning("Cannot subscribe - RabbitMQ connection is not available");
                return;
            }

            try
            {
                // EventingBasicConsumer is in RabbitMQ.Client.Events
                var consumer = new EventingBasicConsumer(_channel!);

                consumer.Received += (_, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    try
                    {
                        messageHandler(message);
                        _channel.BasicAck(ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message");
                        // Requeue the message by setting requeue:true
                        _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                _channel.BasicConsume(
                    queue: _options.QueueName,
                    autoAck: false,
                    consumer: consumer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe to messages");
                throw;
            }
        }

        private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            _logger.LogWarning(
                "RabbitMQ connection shut down: {Reason}",
                e.ReplyText);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _channel?.Dispose();
            _connection?.Dispose();
            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}

