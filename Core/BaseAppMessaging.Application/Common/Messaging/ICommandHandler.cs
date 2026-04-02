namespace BaseAppMessaging.Application.Common.Messaging;

using MediatR;

using BaseAppMessaging.Domain.Primitives;


public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}

public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}
