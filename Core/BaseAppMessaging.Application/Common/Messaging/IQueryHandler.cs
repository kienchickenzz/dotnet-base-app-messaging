namespace BaseAppMessaging.Application.Common.Messaging;

using BaseAppMessaging.Domain.Primitives;

using MediatR;


public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
