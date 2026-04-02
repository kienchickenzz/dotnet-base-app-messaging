namespace BaseAppMessaging.Application.Common.Messaging;

using MediatR;

using BaseAppMessaging.Domain.Primitives;


public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
