/**
 * Messaging configuration settings.
 *
 * <p>Contains provider selection and provider-specific configurations
 * for message broker integration (RabbitMQ, Kafka, etc.).</p>
 */

namespace BaseAppMessaging.Infrastructure.Settings;

/// <summary>
/// Message broker provider options.
/// </summary>
public enum MessagingProviderEnum
{
    /// <summary>
    /// Fake provider for development (log only, no actual sending).
    /// </summary>
    Fake,

    /// <summary>
    /// RabbitMQ message broker.
    /// </summary>
    RabbitMQ,

    /// <summary>
    /// Apache Kafka message broker.
    /// </summary>
    Kafka
}

/// <summary>
/// Root messaging configuration.
/// </summary>
public class MessagingSettings
{
    public const string SectionName = "MessagingSettings";

    /// <summary>
    /// Active messaging provider.
    /// </summary>
    public MessagingProviderEnum Provider { get; set; } = MessagingProviderEnum.Fake;

    /// <summary>
    /// RabbitMQ provider configuration.
    /// </summary>
    public RabbitMQSettings? RabbitMQ { get; set; }

    /// <summary>
    /// Kafka provider configuration.
    /// </summary>
    public KafkaSettings? Kafka { get; set; }
}

/// <summary>
/// RabbitMQ provider configuration.
/// </summary>
public class RabbitMQSettings
{
    /// <summary>
    /// RabbitMQ server hostname.
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// Authentication username.
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// Authentication password.
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Exchange name for publishing messages.
    /// </summary>
    public string ExchangeName { get; set; } = "base-app-messaging";

    /// <summary>
    /// Routing keys for different message types.
    /// Key: message type name, Value: routing key.
    /// </summary>
    public Dictionary<string, string>? RoutingKeys { get; set; }

    /// <summary>
    /// Consumer-specific configurations.
    /// Key: consumer class name, Value: consumer settings.
    /// </summary>
    public Dictionary<string, ConsumerSettings>? Consumers { get; set; }

    /// <summary>
    /// Enable message encryption (AES-256).
    /// </summary>
    public bool MessageEncryptionEnabled { get; set; } = false;

    /// <summary>
    /// Base64-encoded encryption key for AES-256.
    /// </summary>
    public string? MessageEncryptionKey { get; set; }

    /// <summary>
    /// Gets the AMQP connection string.
    /// </summary>
    public string ConnectionString => $"amqp://{UserName}:{Password}@{HostName}/%2f";

    /// <summary>
    /// Gets the routing key for a message type.
    /// </summary>
    public string GetRoutingKey(string messageType, string defaultKey = "default")
    {
        if (RoutingKeys != null && RoutingKeys.TryGetValue(messageType, out var key))
            return key;
        return defaultKey;
    }

    /// <summary>
    /// Gets consumer settings by consumer class name.
    /// </summary>
    public ConsumerSettings GetConsumerSettings(string consumerName)
    {
        if (Consumers != null && Consumers.TryGetValue(consumerName, out var settings))
            return settings;
        return new ConsumerSettings();
    }
}

/// <summary>
/// Consumer-specific settings for RabbitMQ receiver.
/// </summary>
public class ConsumerSettings
{
    /// <summary>
    /// Queue name for this consumer.
    /// </summary>
    public string QueueName { get; set; } = "default-queue";

    /// <summary>
    /// Routing key to bind queue to exchange.
    /// </summary>
    public string RoutingKey { get; set; } = "#";

    /// <summary>
    /// Auto-create queue and bindings if not exist.
    /// </summary>
    public bool AutomaticCreateEnabled { get; set; } = true;

    /// <summary>
    /// Queue type: null (standard), "Quorum", or "Stream".
    /// </summary>
    public string? QueueType { get; set; }

    /// <summary>
    /// Enable single active consumer pattern (only one consumer processes at a time).
    /// </summary>
    public bool SingleActiveConsumer { get; set; } = false;

    /// <summary>
    /// Maximum retry attempts before sending to dead-letter queue.
    /// </summary>
    public int MaxRetryCount { get; set; } = 0;

    /// <summary>
    /// Retry intervals in seconds (e.g., [5, 30, 60]).
    /// </summary>
    public int[]? RetryIntervals { get; set; }

    /// <summary>
    /// Enable dead-letter queue for failed messages.
    /// </summary>
    public bool DeadLetterEnabled { get; set; } = false;
}

/// <summary>
/// Kafka provider configuration.
/// </summary>
public class KafkaSettings
{
    /// <summary>
    /// Kafka bootstrap servers (comma-separated).
    /// </summary>
    public string BootstrapServers { get; set; } = "localhost:9092";

    /// <summary>
    /// Default topic for publishing messages.
    /// </summary>
    public string Topic { get; set; } = "base-app-messaging";

    /// <summary>
    /// Consumer group ID.
    /// </summary>
    public string GroupId { get; set; } = "base-app-consumer";

    /// <summary>
    /// Enable auto commit for consumer.
    /// </summary>
    public bool EnableAutoCommit { get; set; } = true;
}
