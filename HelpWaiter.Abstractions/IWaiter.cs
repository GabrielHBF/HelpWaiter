using HelpWaiter.Abstractions.Command;
using HelpWaiter.Abstractions.Query;

namespace HelpWaiter.Abstractions;

public interface IWaiter
{ 
    Task<TResponse> SendCommandAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    Task<TResponse> AskQueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}