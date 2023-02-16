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
            var result = containerBuilder.BuildServiceProvider(validateScopes: true)
                ;

            return result.CreateAsyncScope().ServiceProvider
                .Register(
                    typeof(TestAssemblyMarker),
                    typeof(CqrsEsAssemblyMarker))
                ;
            ////return result;
        }
    }
}