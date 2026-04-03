/**
 * RabbitMQ implementation of IMessageSender.
 *
 * <p>Publishes messages to RabbitMQ exchange with optional encryption.
 * Uses singleton connection for efficiency.</p>
 */

namespace BaseAppMessaging.Infrastructure.Messaging.RabbitMQClient;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

using BaseAppMessaging.Application.Common.ApplicationServices.Messaging;
using BaseAppMessaging.Infrastructure.Settings;


/// <summary>
/// RabbitMQ implementation of message sender.
/// </summary>
/// <typeparam name="T">The type of message payload.</typeparam>
public class RabbitMQSender<T> : IMessageSender<T> where T : class
{
    private readonly IConnection _connection;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQSender<T>> _logger;

    public RabbitMQSender(
        IConnection connection,
        IOptions<RabbitMQSettings> settings,
        ILogger<RabbitMQSender<T>> logger)
    {
        _connection = connection;
        _settings = settings.Value;
        _logger = logger;

        _logger.LogInformation(
            "[Messaging] Initialized RabbitMQSender | Provider: RabbitMQ | MessageType: {MessageType}",
            typeof(T).Name);
    }

    /// <inheritdoc />
    public async Task SendAsync(T message, MetaData? metaData = null, CancellationToken cancellationToken = default)
    {
        // Create channel per send (channels are not thread-safe)
        await using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        // Declare exchange (idempotent - safe to call multiple times)
        await channel.ExchangeDeclareAsync(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var messageType = typeof(T).Name;
        var routingKey = _settings.GetRoutingKey(messageType);

        // Wrap message with metadata
        var wrappedMessage = new Message<T>
        {
            Data = message,
            MetaData = metaData
        };

        var body = wrappedMessage.GetBytes();

        var properties = new BasicProperties
        {
            Persistent = true,
            MessageId = metaData?.MessageId ?? Guid.NewGuid().ToString(),
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            Type = messageType
        };

        // TODO: Add encryption support if needed
        // if (_settings.MessageEncryptionEnabled && !string.IsNullOrEmpty(_settings.MessageEncryptionKey))
        // {
        //     // Encryption logic here
        // }

        await channel.BasicPublishAsync(
            exchange: _settings.ExchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "[RabbitMQSender] Published | Type: {MessageType} | Exchange: {Exchange} | RoutingKey: {RoutingKey} | MessageId: {MessageId}",
            messageType,
            _settings.ExchangeName,
            routingKey,
            properties.MessageId);
    }
}
