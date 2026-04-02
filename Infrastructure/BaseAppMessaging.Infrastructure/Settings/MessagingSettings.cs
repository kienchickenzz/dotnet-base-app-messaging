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
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// RabbitMQ server port.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// Authentication username.
    /// </summary>
    public string Username { get; set; } = "guest";

    /// <summary>
    /// Authentication password.
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Virtual host name.
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// Exchange name for publishing messages.
    /// </summary>
    public string Exchange { get; set; } = "base-app-messaging";

    /// <summary>
    /// Default queue name for consuming messages.
    /// </summary>
    public string Queue { get; set; } = "default-queue";
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
