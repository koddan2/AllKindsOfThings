using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N2.Domain.DcCase.Events;
using N2.EventSourcing;
namespace N2.Api.Core;

public static class N2ApiApplicationHostExtensions
{
	public static void AddN2ApiApplication(this IServiceCollection services, IConfigurationSection configuration)
	{
		services
			.AddTransient<CaseAggregateEventReader>()
			.AddTransient<N2Facade>()
			.AddN2EventSourcing(configuration)
			.RegisterN2Events(typeof(CaseCreated).Assembly)
			;
	}
}
