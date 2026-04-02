using BaseAppMessaging.Domain.Events;

namespace BaseAppMessaging.Domain.Abstractions;

/// <summary>
/// Defines a contract for entities that can raise domain events.
/// </summary>
/// <remarks>
/// Domain events represent something significant that happened in the domain.
/// They enable loose coupling between domain components and support the outbox pattern
/// for reliable event publishing.
/// </remarks>
public interface IHasDomainEvents
{
    /// <summary>
    /// Gets all pending domain events that have been raised but not yet dispatched.
    /// </summary>
    /// <returns>A read-only list of pending domain events.</returns>
    IReadOnlyList<IDomainEvent> GetDomainEvents();

    /// <summary>
    /// Clears all pending domain events after they have been dispatched.
    /// </summary>
    void ClearDomainEvents();
}
