namespace BaseAppMessaging.Domain.Primitives;

using BaseAppMessaging.Domain.Abstractions;
using BaseAppMessaging.Domain.Events;

/// <summary>
/// Base class for aggregate roots that can raise domain events.
/// </summary>
/// <remarks>
/// Aggregate roots encapsulate a cluster of domain objects and enforce invariants.
/// They are responsible for raising domain events when significant state changes occur.
/// </remarks>
public abstract class BaseEntity : Entity, IAggregateRoot, IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <inheritdoc />
    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    /// <inheritdoc />
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Raises a domain event to be dispatched after the aggregate is persisted.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise.</param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
