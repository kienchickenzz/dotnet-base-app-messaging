namespace BaseAppMessaging.Application.Common.Messaging;

using MediatR;

using BaseAppMessaging.Domain.Events;


public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
