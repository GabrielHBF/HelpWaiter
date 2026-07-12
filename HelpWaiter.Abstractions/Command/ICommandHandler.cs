using HelpWaiter.Abstractions.Base;

namespace HelpWaiter.Abstractions.Command;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
	where TCommand : ICommand<TResponse>;