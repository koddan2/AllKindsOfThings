using N3.App.Domän.Api.Web.Controllers;
using Rebus.Bus;

namespace N3.App.Domän.Api.Web
{
    internal class BusAccumulator : IHostedService
    {
        private readonly IServiceProvider _services;

        public BusAccumulator(IServiceProvider services)
        {
            _services = services;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _services.CreateAsyncScope();
            var bus = scope.ServiceProvider.GetRequiredService<IBus>();
            await bus.Subscribe<ImporteraInkassoÄrendeModell>();

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}