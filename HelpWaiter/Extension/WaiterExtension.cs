using HelpWaiter.Abstractions;
using HelpWaiter.Abstractions.Base;
using HelpWaiter.Abstractions.Command;
using HelpWaiter.Abstractions.Query;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HelpWaiter.Extension;

public static class WaiterExtension
{
	public static IServiceCollection AddWaiter(this IServiceCollection services,
		params Assembly[] assemblies)
	{
		services.AddTransient<IWaiter, Waiter>();

		var handlerTypes = new[]
		{
			typeof(IRequestHandler<,>),
			typeof(ICommandHandler<,>),
			typeof(IQueryHandler<,>)
		};

		foreach (var assembly in assemblies)
		{
			var handlers = assembly.GetTypes()
				.Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition)
				.SelectMany(type => type.GetInterfaces(), (t, i) => new { type = t, iface = i })
				.Where(type => type.iface.IsGenericType && handlerTypes.Contains(type.iface.GetGenericTypeDefinition()));

			foreach (var handler in handlers)
			{
				services.AddTransient(handler.iface, handler.type);
			}
		}

		return services;
	}
}