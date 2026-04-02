namespace BaseAppMessaging.Application.Common.Messaging;

using MediatR;

using BaseAppMessaging.Domain.Primitives;


public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
