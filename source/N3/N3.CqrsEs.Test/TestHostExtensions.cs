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
                   _ = services
                       ;

                   foreach (var type in types)
                   {
                   }
               });
        }

        ////public static ILifetimeSelector ResolveHandlers(this IImplementationTypeSelector selector, Type _)
        ////{
        ////    return selector
        ////        .AddClasses(classes => classes.Where(x =>
        ////        {
        ////            var allInterfaces = x.GetInterfaces();
        ////            return
        ////                allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICancellableQueryHandler<,>));
        ////        }))
        ////        .AsSelf();
        ////}

        public static T Plocka<T>(this IServiceScope scope)
            where T : notnull
        {
            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }
}