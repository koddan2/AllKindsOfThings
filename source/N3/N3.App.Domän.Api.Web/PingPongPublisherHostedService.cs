using N3.CqrsEs.SkrivModell.JobbPaket;
using SlimMessageBus;

namespace N3.App.Domän.Api.Web
{
    internal class PingPongPublisherHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public PingPongPublisherHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            while (!cancellationToken.IsCancellationRequested)
            {
                await bus.Publish(
                    new PingPongMessage { MessageText = "ping" },
                    cancellationToken: cancellationToken
                );
                await Task.Delay(1500, cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await ValueTask.CompletedTask;
        }
    }
}
