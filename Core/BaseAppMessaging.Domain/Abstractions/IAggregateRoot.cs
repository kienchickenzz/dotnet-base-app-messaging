namespace BaseAppMessaging.Domain.Abstractions;

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
