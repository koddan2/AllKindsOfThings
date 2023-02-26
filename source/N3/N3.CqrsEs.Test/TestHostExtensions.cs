using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace N3.CqrsEs.Test
{
    public static class TestHostExtensions
    {
        public static IHostBuilder RegisterCqrsServices(this IHostBuilder host, params Type[] types)
        {
            return host.ConfigureServices(
                (ctx, services) =>
                {
                    _ = services;

                    foreach (var type in types) { }
                }
            );
        }

        public static T Plocka<T>(this IServiceScope scope)
            where T : notnull
        {
            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }
}
