using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Events;
using CQRSlite.Messages;
using CQRSlite.Queries;
using CQRSlite.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N3.CqrsEs.Test.TestTjänster;
using Scrutor;
using System.Reflection;

namespace N3.CqrsEs.Test
{
    public static class TestHostExtensions
    {
        public static IHostBuilder RegisterCqrsServices(this IHostBuilder host, params Type[] types)
        {
            return host
               .ConfigureServices((ctx, services) =>
               {
                   var router = new Router();
                   _ = services
                       .AddSingleton<Router>(router)
                       .AddSingleton<ICommandSender>(_ => router)
                       .AddSingleton<IEventPublisher>(_ => router)
                       .AddSingleton<IHandlerRegistrar>(_ => router)
                       .AddSingleton<IQueryProcessor>(_ => router)
                       .AddScoped<ISession, Session>()

                       .AddSingleton<IEventStore, InMemoryEventStore>()
                       .AddScoped<IRepository>(y => new Repository(y.GetRequiredService<IEventStore>()))
                       ;

                   foreach (var type in types)
                   {
                       _ = services.Scan(scan => scan
                            .FromAssemblies(type.GetTypeInfo().Assembly)
                                .ResolveHandlers(type)
                                .WithTransientLifetime()
                        );
                   }
               });
        }

        public static ILifetimeSelector ResolveHandlers(this IImplementationTypeSelector selector, Type _)
        {
            return selector
                .AddClasses(classes => classes.Where(x =>
                {
                    var allInterfaces = x.GetInterfaces();
                    return
                        allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(IHandler<>)) ||
                        allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICancellableHandler<>)) ||
                        allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(IQueryHandler<,>)) ||
                        allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICancellableQueryHandler<,>));
                }))
                .AsSelf();
        }
    }
}