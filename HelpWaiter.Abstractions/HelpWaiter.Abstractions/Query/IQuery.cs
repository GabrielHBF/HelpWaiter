using HelpWaiter.Abstractions.Base;

namespace HelpWaiter.Abstractions.Query;

public interface IQuery<out TResponse> : IRequest<TResponse>;