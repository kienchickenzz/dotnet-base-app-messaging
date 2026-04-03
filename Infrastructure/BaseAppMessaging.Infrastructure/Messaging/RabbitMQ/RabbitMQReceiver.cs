/**
 * RabbitMQ implementation of IMessageReceiver.
 *
 * <p>Consumes messages from RabbitMQ queue with support for:
 * - Auto queue/exchange creation
 * - Dead-letter queue
 * - Retry with exponential backoff
 * - Single active consumer pattern</p>
 */

namespace BaseAppMessaging.Infrastructure.Messaging.RabbitMQClient;

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using BaseAppMessaging.Application.Common.ApplicationServices.Messaging;
using BaseAppMessaging.Infrastructure.Settings;


/// <summary>
/// RabbitMQ implementation of message receiver.
/// </summary>
/// <typeparam name="TConsumer">The consumer type for routing configuration.</typeparam>
/// <typeparam name="T">The type of message payload.</typeparam>
public class RabbitMQReceiver<TConsumer, T> : IMessageReceiver<TConsumer, T>, IDisposable where T : class
{
    private readonly IConnection _connection;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQReceiver<TConsumer, T>> _logger;
    private IChannel? _channel;

    public RabbitMQReceiver(
        IConnection connection,
        IOptions<RabbitMQSettings> settings,
        ILogger<RabbitMQReceiver<TConsumer, T>> logger)
    {
        _connection = connection;
        _settings = settings.Value;
        _logger = logger;

        _logger.LogInformation(
            "[Messaging] Initialized RabbitMQReceiver | Provider: RabbitMQ | Consumer: {Consumer} | MessageType: {MessageType}",
            typeof(TConsumer).Name,
            typeof(T).Name);
    }

    /// <inheritdoc />
    public async Task ReceiveAsync(Func<T, MetaData?, CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        var consumerName = typeof(TConsumer).Name;
        var consumerSettings = _settings.GetConsumerSettings(consumerName);

        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        // Setup queue infrastructure if enabled
        if (consumerSettings.AutomaticCreateEnabled)
        {
            await _SetupQueueInfrastructureAsync(consumerSettings, cancellationToken);
        }

        // Set QoS: process one message at a time
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            try
            {
                var bodyText = Encoding.UTF8.GetString(ea.Body.Span);

                // TODO: Add decryption support if needed
                // if (IsEncrypted(ea.BasicProperties))
                // {
                //     // Decryption logic here
                // }

                var message = JsonSerializer.Deserialize<Message<T>>(bodyText);

                if (message?.Data != null)
                {
                    await action(message.Data, message.MetaData, cancellationToken);
                }

                // Acknowledge successful processing
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                _logger.LogInformation(
                    "[RabbitMQReceiver] Processed | Consumer: {Consumer} | MessageType: {MessageType} | MessageId: {MessageId}",
                    consumerName,
                    typeof(T).Name,
                    message?.MetaData?.MessageId ?? "N/A");
            }
            catch (ConsumerHandledException ex)
            {
                await _HandleConsumerExceptionAsync(ea, consumerSettings, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[RabbitMQReceiver] Error processing message | Consumer: {Consumer} | Queue: {Queue}",
                    consumerName,
                    consumerSettings.QueueName);

                // Nack without requeue to avoid infinite loop
                await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: consumerSettings.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "[RabbitMQReceiver] Started consuming | Consumer: {Consumer} | Queue: {Queue}",
            consumerName,
            consumerSettings.QueueName);

        // Keep running until cancellation
        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation(
                "[RabbitMQReceiver] Stopping | Consumer: {Consumer}",
                consumerName);
        }
    }

    /// <summary>
    /// Setup queue, exchange bindings, dead-letter queue, and retry queues.
    /// </summary>
    private async Task _SetupQueueInfrastructureAsync(ConsumerSettings settings, CancellationToken cancellationToken)
    {
        var arguments = new Dictionary<string, object?>();

        // Queue type (Quorum, Stream)
        if (!string.IsNullOrEmpty(settings.QueueType))
        {
            if (string.Equals(settings.QueueType, "Quorum", StringComparison.OrdinalIgnoreCase))
                arguments["x-queue-type"] = "quorum";
            else if (string.Equals(settings.QueueType, "Stream", StringComparison.OrdinalIgnoreCase))
                arguments["x-queue-type"] = "stream";
        }

        // Single active consumer
        if (settings.SingleActiveConsumer)
        {
            arguments["x-single-active-consumer"] = true;
        }

        // Dead-letter queue
        if (settings.DeadLetterEnabled)
        {
            var dlqName = settings.QueueName + "-dead-letters";
            arguments["x-dead-letter-exchange"] = string.Empty;
            arguments["x-dead-letter-routing-key"] = dlqName;

            await _channel!.QueueDeclareAsync(
                queue: dlqName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            _logger.LogInformation("[RabbitMQReceiver] Created DLQ: {DlqName}", dlqName);
        }

        // Retry queues with TTL
        if (settings.MaxRetryCount > 0 && settings.RetryIntervals != null)
        {
            foreach (var intervalInSeconds in settings.RetryIntervals)
            {
                var retryQueueName = $"{settings.QueueName}-retry-{intervalInSeconds}";
                var retryArgs = new Dictionary<string, object?>
                {
                    { "x-message-ttl", intervalInSeconds * 1000 },
                    { "x-dead-letter-exchange", string.Empty },
                    { "x-dead-letter-routing-key", settings.QueueName }
                };

                await _channel!.QueueDeclareAsync(
                    queue: retryQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: retryArgs,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("[RabbitMQReceiver] Created retry queue: {RetryQueue}", retryQueueName);
            }
        }

        // Declare main queue
        await _channel!.QueueDeclareAsync(
            queue: settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments.Count > 0 ? arguments : null,
            cancellationToken: cancellationToken);

        // Bind queue to exchange
        await _channel.QueueBindAsync(
            queue: settings.QueueName,
            exchange: _settings.ExchangeName,
            routingKey: settings.RoutingKey,
            arguments: null,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "[RabbitMQReceiver] Queue ready | Queue: {Queue} | Exchange: {Exchange} | RoutingKey: {RoutingKey}",
            settings.QueueName,
            _settings.ExchangeName,
            settings.RoutingKey);
    }

    /// <summary>
    /// Handle ConsumerHandledException with retry/requeue/dead-letter logic.
    /// </summary>
    private async Task _HandleConsumerExceptionAsync(
        BasicDeliverEventArgs ea,
        ConsumerSettings settings,
        ConsumerHandledException ex)
    {
        switch (ex.NextAction)
        {
            case ConsumerHandledExceptionNextAction.Retry:
                if (settings.MaxRetryCount > 0 && settings.RetryIntervals != null)
                {
                    var retryCount = _GetRetryCount(ea.BasicProperties);

                    if (retryCount < settings.MaxRetryCount)
                    {
                        var retryInterval = _GetRetryInterval(retryCount + 1, settings.RetryIntervals);
                        var retryQueue = $"{settings.QueueName}-retry-{retryInterval}";

                        var props = new BasicProperties
                        {
                            Persistent = true,
                            Headers = ea.BasicProperties.Headers ?? new Dictionary<string, object?>()
                        };
                        props.Headers["x-retry"] = retryCount + 1;

                        await _channel!.BasicPublishAsync(
                            exchange: string.Empty,
                            routingKey: retryQueue,
                            mandatory: true,
                            basicProperties: props,
                            body: ea.Body.ToArray());

                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                        _logger.LogWarning(
                            "[RabbitMQReceiver] Retry scheduled | Attempt: {Attempt}/{Max} | RetryQueue: {RetryQueue}",
                            retryCount + 1,
                            settings.MaxRetryCount,
                            retryQueue);
                        return;
                    }
                }

                // Max retries exceeded, send to DLQ
                if (settings.DeadLetterEnabled)
                {
                    await _channel!.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    _logger.LogError("[RabbitMQReceiver] Max retries exceeded, sent to DLQ");
                }
                break;

            case ConsumerHandledExceptionNextAction.ReQueue:
                await _channel!.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                _logger.LogWarning("[RabbitMQReceiver] Message requeued");
                break;

            case ConsumerHandledExceptionNextAction.DeadLetter:
                if (settings.DeadLetterEnabled)
                {
                    await _channel!.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    _logger.LogError("[RabbitMQReceiver] Message sent to DLQ");
                }
                break;
        }
    }

    private static int _GetRetryCount(IReadOnlyBasicProperties props)
    {
        if (props?.Headers != null && props.Headers.TryGetValue("x-retry", out var val))
        {
            if (val is byte[] bytes)
                return int.Parse(Encoding.UTF8.GetString(bytes));
            return Convert.ToInt32(val);
        }
        return 0;
    }

    private static int _GetRetryInterval(int retryCount, int[] intervals)
    {
        var index = (retryCount - 1) % intervals.Length;
        return intervals[index];
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}
