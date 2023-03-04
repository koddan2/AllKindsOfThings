using Microsoft.Extensions.Hosting;
using N3.CqrsEs.Messages;
using N3.CqrsEs.SkrivModell.JobbPaket;
using Rebus.Bus;

namespace N3.App.Domän.Api.Web
{
    internal class PingPongPublisherBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public PingPongPublisherBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var bus = scope.ServiceProvider.GetRequiredService<IBus>();
            while (!cancellationToken.IsCancellationRequested)
            {
                await bus.Publish(new PingPongMessage { MessageText = "ping" });
                var timeout = cfg.GetValue("PingPongTimeout", TimeSpan.FromSeconds(30));
                await Task.Delay(timeout, cancellationToken);
            }
        }
    }
}
