using MediatR;

namespace BaseAppMessaging.Domain.Events;

/// <summary>
/// Marker interface for domain events.
/// </summary>
/// <remarks>
/// Domain events represent facts about something that happened in the domain.
/// They are immutable and named in past tense (e.g., OrderCreated, PaymentReceived).
/// Implements <see cref="INotification"/> to enable publishing via MediatR.
/// </remarks>
public interface IDomainEvent : INotification
{
}
