using N3.CqrsEs.Messages;
using Rebus.Bus;

namespace N3.App.Ombud.Web
{
    internal class BootstrapperBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BootstrapperBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            var logger = scope.ServiceProvider.GetRequiredService<
                ILogger<BootstrapperBackgroundService>
            >();
            while (!cancellationToken.IsCancellationRequested)
            {
                //var _ = scope.ServiceProvider.GetRequiredService<IBus>();
                logger.LogInformation("OK");
                await Task.Delay(10000, cancellationToken);
            }

            logger.LogInformation("Background service exiting.");
        }
    }
}
