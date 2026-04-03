namespace BaseAppMessaging.Infrastructure.Messaging.Fake;

using Microsoft.Extensions.Logging;

using BaseAppMessaging.Application.Common.ApplicationServices.Messaging;

/// <summary>
/// No-op implementation of IMessageSender for testing and development.
/// </summary>
/// <typeparam name="T">The type of message payload.</typeparam>
public class FakeSender<T> : IMessageSender<T> where T : class
{
    private readonly ILogger<FakeSender<T>> _logger;

    public FakeSender(ILogger<FakeSender<T>> logger)
    {
        _logger = logger;

        _logger.LogInformation(
            "[Messaging] Initialized FakeSender | Provider: Fake | MessageType: {MessageType}",
            typeof(T).Name);
    }

    /// <inheritdoc />
    public Task SendAsync(T message, MetaData? metaData = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[FakeSender] Publishing message | Type: {MessageType} | MessageId: {MessageId} | ActivityId: {ActivityId}",
            typeof(T).Name,
            metaData?.MessageId ?? "N/A",
            metaData?.ActivityId ?? "N/A");

        return Task.CompletedTask;
    }
}
