using HelpWaiter.Abstractions.Base;

namespace HelpWaiter.Abstractions.Query;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;