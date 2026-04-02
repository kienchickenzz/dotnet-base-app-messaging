using BaseAppMessaging.Domain.Abstractions;
using BaseAppMessaging.Domain.Events;

namespace BaseAppMessaging.Domain.Primitives;

/// <summary>
/// Marker interface for aggregate roots in the domain.
/// </summary>
/// <remarks>
/// Aggregate roots are the entry points to aggregates and are the only entities
/// that external objects can hold references to. They ensure consistency boundaries
/// within the domain.
/// </remarks>
public interface IAggregateRoot
{
}

/// <summary>
/// Base class for aggregate roots that can raise domain events.
/// </summary>
/// <remarks>
/// Aggregate roots encapsulate a cluster of domain objects and enforce invariants.
/// They are responsible for raising domain events when significant state changes occur.
/// </remarks>
public abstract class AggregateRoot : Entity, IAggregateRoot, IHasDomainEvents
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
