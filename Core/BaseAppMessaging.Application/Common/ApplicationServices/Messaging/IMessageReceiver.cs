namespace BaseAppMessaging.Application.Common.ApplicationServices.Messaging;

/// <summary>
/// Defines a contract for receiving messages from a message broker.
/// </summary>
/// <typeparam name="TConsumer">The consumer type for routing/grouping purposes.</typeparam>
/// <typeparam name="T">The type of message payload to receive.</typeparam>
public interface IMessageReceiver<TConsumer, T> where T : class
{
    /// <summary>
    /// Starts receiving messages and invokes the action for each received message.
    /// </summary>
    /// <param name="action">The handler to process received messages.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task ReceiveAsync(Func<T, MetaData?, CancellationToken, Task> action, CancellationToken cancellationToken);
}

/// <summary>
/// Exception thrown when a consumer fails to handle a message.
/// </summary>
public class ConsumerHandledException : Exception
{
    public ConsumerHandledException() { }

    public ConsumerHandledException(string message) : base(message) { }

    public ConsumerHandledException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Specifies what action to take after the exception is caught.
    /// </summary>
    public ConsumerHandledExceptionNextAction NextAction { get; set; }
}

/// <summary>
/// Defines the next action to take when a consumer fails to handle a message.
/// </summary>
public enum ConsumerHandledExceptionNextAction
{
    /// <summary>
    /// Retry processing the message immediately.
    /// </summary>
    Retry,

    /// <summary>
    /// Re-queue the message for later processing.
    /// </summary>
    ReQueue,

    /// <summary>
    /// Send the message to the dead-letter queue.
    /// </summary>
    DeadLetter,
}
