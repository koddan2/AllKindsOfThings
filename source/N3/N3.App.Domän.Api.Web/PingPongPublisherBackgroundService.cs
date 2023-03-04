using Microsoft.Extensions.Hosting;
using N3.CqrsEs.Messages;
using N3.CqrsEs.SkrivModell.JobbPaket;
using SlimMessageBus;

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
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            while (!cancellationToken.IsCancellationRequested)
            {
                await bus.Publish(
                    new PingPongMessage { MessageText = "ping" },
                    cancellationToken: cancellationToken
                );
                await bus.Publish(
                    new ImportAvInkassoÄrendeKölagt { JobbId = "ping" },
                    cancellationToken: cancellationToken
                );
                await Task.Delay(15000, cancellationToken);
            }
        }
    }
}
