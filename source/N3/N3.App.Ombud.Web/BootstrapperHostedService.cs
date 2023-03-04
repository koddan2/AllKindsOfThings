using SlimMessageBus;
using SlimMessageBus.Host;
using SlimMessageBus.Host.Redis;

namespace N3.App.Ombud.Web
{
    internal class BootstrapperHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public BootstrapperHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            while (!cancellationToken.IsCancellationRequested)
            {
                var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
                var cc = scope.ServiceProvider.GetRequiredService<IConsumerControl>();
                if (cc.IsStarted)
                {
                    var a = 1;
                }
                else
                {
                    var b = 2;
                }
                await Task.Delay(1000, cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await ValueTask.CompletedTask;
        }
    }
}
