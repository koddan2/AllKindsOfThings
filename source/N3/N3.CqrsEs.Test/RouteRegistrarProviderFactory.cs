using Microsoft.Extensions.DependencyInjection;

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
            var result = containerBuilder.BuildServiceProvider()
                .Register(
                    typeof(TestAssemblyMarker),
                    typeof(CqrsEsAssemblyMarker));

            return result;
        }
    }
}