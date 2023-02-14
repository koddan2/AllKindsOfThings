using CQRSlite.Routing;
using Microsoft.Extensions.DependencyInjection;
using N3.CqrsEs.SkrivModell.Hantering;

namespace N3.CqrsEs.Test
{
    public class RouteRegistrarProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            var serviceProvider = containerBuilder.BuildServiceProvider();
            var registrar = new RouteRegistrar(new TestProvider(serviceProvider));
            registrar.RegisterInAssemblyOf(typeof(InkassoÄrendeKommandoHanterare));
            return serviceProvider;
        }
    }
}