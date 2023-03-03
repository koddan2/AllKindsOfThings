using N3.App.Domän.Api.Web.Controllers;

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
