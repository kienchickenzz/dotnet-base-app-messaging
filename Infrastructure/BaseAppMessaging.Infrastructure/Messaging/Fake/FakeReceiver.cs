namespace BaseAppMessaging.Infrastructure.Messaging.Fake;

using Microsoft.Extensions.Logging;

using BaseAppMessaging.Application.Common.ApplicationServices.Messaging;

/// <summary>
/// No-op implementation of IMessageReceiver for testing and development.
/// </summary>
/// <typeparam name="TConsumer">The consumer type.</typeparam>
/// <typeparam name="T">The type of message payload.</typeparam>
public class FakeReceiver<TConsumer, T> : IMessageReceiver<TConsumer, T>
{
    private readonly ILogger<FakeReceiver<TConsumer, T>> _logger;

    public FakeReceiver(ILogger<FakeReceiver<TConsumer, T>> logger)
    {
        _logger = logger;

        _logger.LogInformation(
            "[Messaging] Initialized FakeReceiver | Provider: Fake | Consumer: {Consumer} | MessageType: {MessageType}",
            typeof(TConsumer).Name,
            typeof(T).Name);
    }

    /// <inheritdoc />
    public Task ReceiveAsync(Func<T, MetaData?, CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[FakeReceiver] Listening for messages | Type: {MessageType} | Consumer: {ConsumerType}",
            typeof(T).Name,
            typeof(TConsumer).Name);

        // No-op: no messages are received
        return Task.CompletedTask;
    }
}
