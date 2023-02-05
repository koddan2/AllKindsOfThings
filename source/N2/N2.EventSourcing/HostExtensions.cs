using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N2.Domain;
using N2.Domain.DcCase;
using N2.EventSourcing.Common;
using System.Collections.Concurrent;
using System.Reflection;

namespace N2.EventSourcing
{
	public static class HostExtensions
	{
		public static IServiceCollection AddN2EventSourcing(this IServiceCollection services, IConfigurationSection configuration)
		{
			return services
				.AddTransient<CaseAggregateCommandHandler>()
				.Configure<N2EventSourcingConfiguration>(configuration)
				.AddTransient<IEventSender, EsdbStore>()
				.AddTransient<IEventReader, EsdbStore>()
				;
		}

		public static void RegisterN2Events(this IServiceCollection services, Assembly assembly)
		{
			var eventsDict = new ConcurrentDictionary<string, Type>();
			var registry = new N2Registry(eventsDict);
			services.AddSingleton<N2Registry>(registry);

			foreach (var type in assembly.DefinedTypes)
			{
				if (type.GetCustomAttribute<N2EventAttribute>() is not null)
				{
					ValidateType(type);
					eventsDict[type.Name] = type;
				}
			}
		}

		private static void ValidateType(Type type)
		{
			var assignable = typeof(IEvent).IsAssignableFrom(type);
			if (!assignable)
			{
				throw new ApplicationException($"Type {type.FullName} is not assignable to {typeof(IEvent).FullName}");
			}
		}
	}
}