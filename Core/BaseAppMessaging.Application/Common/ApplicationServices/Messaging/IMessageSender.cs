namespace BaseAppMessaging.Application.Common.ApplicationServices.Messaging;

/// <summary>
/// Defines a contract for sending messages to a message broker.
/// </summary>
/// <typeparam name="T">The type of message payload to send.</typeparam>
public interface IMessageSender<T>
{
    /// <summary>
    /// Sends a message asynchronously to the configured message broker.
    /// </summary>
    /// <param name="message">The message payload to send.</param>
    /// <param name="metaData">Optional metadata for tracing and correlation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task SendAsync(T message, MetaData? metaData = null, CancellationToken cancellationToken = default);
}
