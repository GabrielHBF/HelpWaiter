using HelpWaiter.Abstractions;
using HelpWaiter.Abstractions.Base;
using HelpWaiter.Abstractions.Command;
using HelpWaiter.Abstractions.Query;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace HelpWaiter;

public class Waiter(IServiceProvider serviceProvider) : IWaiter
{
	public Task<TResponse> AskQueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
	{
		return DispatchAsync(query, serviceProvider, cancellationToken);
	}

	public Task<TResponse> SendCommandAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
	{
		return DispatchAsync(command, serviceProvider, cancellationToken);
	}

	private static readonly ConcurrentDictionary<Type, Func<object, object, CancellationToken, Task>> _handlerCache = new();
	private async Task<TResponse> DispatchAsync<TResponse>(
	IRequest<TResponse> request,
	IServiceProvider serviceProvider,
	CancellationToken cancellationToken)
	{
		var requestType = request.GetType();
		var responseType = typeof(TResponse);

		var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
		var handler = serviceProvider.GetService(handlerType);

		if (handler is null)
			throw new InvalidOperationException($"No handler of type '{handlerType.Name}' has been registered in the Dependency Injection container for request '{requestType.Name}'.");

		var executor = _handlerCache.GetOrAdd(requestType, _ => BuildFastExecutor(handlerType, requestType, responseType));

		try
		{
			var task = (Task<TResponse>) executor(handler, request, cancellationToken);
			return await task.ConfigureAwait(false);
		}
		catch (TargetInvocationException ex) when (ex.InnerException is not null)
		{
			System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
			throw;
		}
	}

	private static Func<object, object, CancellationToken, Task> BuildFastExecutor(
		Type handlerType,
		Type requestType,
		Type responseType)
	{
		var methodInfo = handlerType.GetMethod("HandleAsync");
		if (methodInfo is null)
			throw new InvalidOperationException($"Method 'HandleAsync' was not found on interface '{handlerType.Name}'.");

		var handlerParam = Expression.Parameter(typeof(object), "handler");
		var requestParam = Expression.Parameter(typeof(object), "request");
		var tokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

		var handlerCast = Expression.Convert(handlerParam, handlerType);
		var requestCast = Expression.Convert(requestParam, requestType);

		var methodCall = Expression.Call(handlerCast, methodInfo, requestCast, tokenParam);

		var lambda = Expression.Lambda<Func<object, object, CancellationToken, Task>>(
			methodCall, handlerParam, requestParam, tokenParam);

		return lambda.Compile();
	}
}