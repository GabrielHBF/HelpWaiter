using HelpWaiter.Abstractions.Base;

namespace HelpWaiter.Abstractions.Command;

public interface ICommand<TResponse> : IRequest<TResponse>;