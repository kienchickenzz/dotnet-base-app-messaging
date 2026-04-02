/**
 * Consumer for ProductCreatedDomainEvent.
 *
 * <p>Listens to message broker and processes product creation events.
 * Executes side effects like sending emails, updating caches, etc.</p>
 */

namespace BaseAppMessaging.Worker.Consumers.Products;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using BaseAppMessaging.Application.Common.ApplicationServices.Messaging;
using BaseAppMessaging.Domain.AggregatesModels.Products.Events;


/// <summary>
/// Background service that consumes ProductCreatedDomainEvent from message broker.
/// </summary>
public class ProductCreatedConsumer : BackgroundService
{
    private readonly ILogger<ProductCreatedConsumer> _logger;
    private readonly IMessageReceiver<ProductCreatedConsumer, ProductCreatedDomainEvent> _receiver;

    public ProductCreatedConsumer(
        ILogger<ProductCreatedConsumer> logger,
        IMessageReceiver<ProductCreatedConsumer, ProductCreatedDomainEvent> receiver)
    {
        _logger = logger;
        _receiver = receiver;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Consumer] ProductCreatedConsumer started, waiting for messages...");

        try
        {
            await _receiver.ReceiveAsync(async (message, metaData, ct) =>
            {
                _logger.LogInformation(
                    "[Consumer] Received ProductCreated | Product: {ProductName} | Price: {Price:C} | MessageId: {MessageId}",
                    message.ProductName,
                    message.Price,
                    metaData?.MessageId ?? "N/A");

                // Execute side effects
                await _ProcessMessageAsync(message, metaData, ct);

            }, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("[Consumer] ProductCreatedConsumer stopping due to cancellation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Consumer] ProductCreatedConsumer encountered an error");
            throw;
        }

        _logger.LogInformation("[Consumer] ProductCreatedConsumer stopped");
    }

    /// <summary>
    /// Process the received message and execute side effects.
    /// </summary>
    private async Task _ProcessMessageAsync(
        ProductCreatedDomainEvent message,
        MetaData? metaData,
        CancellationToken cancellationToken)
    {
        // TODO: Implement actual side effects here
        // Examples:
        // - Send welcome/notification email
        // - Update search index (Elasticsearch)
        // - Invalidate/update cache
        // - Notify other microservices
        // - Sync to external systems (CRM, analytics)

        _logger.LogInformation(
            "[Consumer] Processing ProductCreated | Product: {ProductName}",
            message.ProductName);

        // Simulate processing time
        await Task.Delay(100, cancellationToken);

        _logger.LogInformation(
            "[Consumer] Completed processing ProductCreated | Product: {ProductName}",
            message.ProductName);
    }
}
