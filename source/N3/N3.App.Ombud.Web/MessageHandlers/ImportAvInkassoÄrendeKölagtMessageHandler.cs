using N3.CqrsEs.SkrivModell.JobbPaket;
using SlimMessageBus;

namespace N3.App.Ombud.Web.MessageHandlers
{
    public class ImportAvInkassoÄrendeKölagtMessageHandler : IConsumer<ImportAvInkassoÄrendeKölagt>
    {
        private readonly ILogger<ImportAvInkassoÄrendeKölagtMessageHandler> _logger;

        public ImportAvInkassoÄrendeKölagtMessageHandler(
            ILogger<ImportAvInkassoÄrendeKölagtMessageHandler> logger
        )
        {
            _logger = logger;
        }

        public async Task OnHandle(ImportAvInkassoÄrendeKölagt message)
        {
            await ValueTask.CompletedTask;
            _logger.LogInformation("Ärende kölagt: {msg}", message);
        }
    }
}
