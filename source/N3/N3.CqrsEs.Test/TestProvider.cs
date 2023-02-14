using Microsoft.Extensions.DependencyInjection;

namespace N3.CqrsEs.Test
{
    public class TestProvider : IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public TestProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object? GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
    }
}