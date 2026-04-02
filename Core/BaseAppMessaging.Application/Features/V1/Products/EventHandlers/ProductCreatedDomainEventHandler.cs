/**
 * Handler for ProductCreatedDomainEvent.
 *
 * <p>Processes the event after a product is successfully created.
 * Publishes event to message broker as a side effect.</p>
 */

namespace BaseAppMessaging.Application.Features.V1.Products.EventHandlers;

using System.Diagnostics;
using Microsoft.Extensions.Logging;

using BaseAppMessaging.Application.Common.Messaging;
using BaseAppMessaging.Application.Common.ApplicationServices.Messaging;
using BaseAppMessaging.Domain.AggregatesModels.Products.Events;


/// <summary>
/// Handles ProductCreatedDomainEvent published via outbox pattern.
/// </summary>
public sealed class ProductCreatedDomainEventHandler : IDomainEventHandler<ProductCreatedDomainEvent>
{
    private readonly ILogger<ProductCreatedDomainEventHandler> _logger;
    private readonly IMessageSender<ProductCreatedDomainEvent> _messageSender;

    public ProductCreatedDomainEventHandler(
        ILogger<ProductCreatedDomainEventHandler> logger,
        IMessageSender<ProductCreatedDomainEvent> messageSender)
    {
        _logger = logger;
        _messageSender = messageSender;
    }

    /// <summary>
    /// Publishes event to message broker when a new product is created.
    /// </summary>
    public async Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[Domain Event] ProductCreated - Name: {ProductName}, Price: {Price:C}",
            notification.ProductName,
            notification.Price);

        try
        {
            // Build metadata for tracing
            var metaData = new MetaData
            {
                MessageId = Guid.NewGuid().ToString(),
                ActivityId = Activity.Current?.Id,
                CreationDateTime = DateTimeOffset.UtcNow
            };

            // Publish to message broker
            await _messageSender.SendAsync(notification, metaData, cancellationToken);

            _logger.LogInformation(
                "[Domain Event] ProductCreated - Published to broker | MessageId: {MessageId}",
                metaData.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "[Domain Event] ProductCreated - Failed to publish | Product: {ProductName}",
                notification.ProductName);
        }
    }
}
