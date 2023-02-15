using CQRSlite.Routing;
using Microsoft.Extensions.DependencyInjection;
using N3.CqrsEs.SkrivModell.Hantering;
using System.Runtime.CompilerServices;

namespace N3.CqrsEs.Test
{
    public static class TestServiceScopeExtensions
    {
        public static IServiceProvider Register(this IServiceProvider serviceProvider, params Type[] types)
        {
            var registrar = new RouteRegistrar(serviceProvider);
            foreach (var type in types)
            {
                registrar.RegisterInAssemblyOf(type);
            }

            return serviceProvider;
        }

        public static IServiceScope Register(this IServiceScope scope, params Type[] types)
        {
            _ = scope.ServiceProvider.Register(types);
            return scope;
        }

        public static T Plocka<T>(this IServiceScope scope)
            where T : notnull
        {
            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }
}