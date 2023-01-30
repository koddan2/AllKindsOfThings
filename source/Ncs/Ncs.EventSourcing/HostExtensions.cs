using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAK;

namespace Ncs.EventSourcing
{
	public static class HostExtensions
	{
		public static IServiceCollection AddNcsEventSourcingEventStoreDb(this IServiceCollection services, IConfigurationSection configuration)
		{
			return services.AddNcsEventSourcingEventStoreDb(configuration.Get<EventStoreDbConfiguration>().OrFail());
		}
		public static IServiceCollection AddNcsEventSourcingEventStoreDb(this IServiceCollection services, EventStoreDbConfiguration configuration)
		{
			return services.AddTransient<EventStoreClient>((c) =>
			{
				var settings = EventStoreClientSettings
					.Create(configuration.ConnectionString);
				var client = new EventStoreClient(settings);
				return client;
			})
				.AddScoped<IEventStore, EventStoreDbMediator>()
				.AddTransient<IUniqueIdGenerator, ShortGuidGenerator>()
				;
		}
	}
}