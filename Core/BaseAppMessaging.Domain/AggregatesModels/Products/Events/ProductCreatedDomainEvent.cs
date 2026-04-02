/**
 * Domain event raised when a new product is created.
 *
 * <p>Captured by outbox pattern and processed asynchronously
 * for notifications, integrations, or audit logging.</p>
 */

namespace BaseAppMessaging.Domain.AggregatesModels.Products.Events;

using BaseAppMessaging.Domain.Events;


/// <summary>
/// Event fired after a product is successfully created.
/// </summary>
/// <param name="ProductName">Name of the newly created product.</param>
/// <param name="Price">Price of the product.</param>
public sealed record ProductCreatedDomainEvent(string ProductName, decimal Price) : IDomainEvent;
