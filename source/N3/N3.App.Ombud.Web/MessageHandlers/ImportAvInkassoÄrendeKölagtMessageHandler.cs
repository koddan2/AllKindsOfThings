using N3.CqrsEs.Messages;
using Rebus.Handlers;

namespace N3.App.Ombud.Web.MessageHandlers
{
    public class ImportAvInkassoÄrendeKölagtMessageHandler
        : IHandleMessages<ImportAvInkassoÄrendeKölagt>
    {
        private readonly ILogger<ImportAvInkassoÄrendeKölagtMessageHandler> _logger;

        public ImportAvInkassoÄrendeKölagtMessageHandler(
            ILogger<ImportAvInkassoÄrendeKölagtMessageHandler> logger
        )
        {
            _logger = logger;
        }

        public async Task Handle(ImportAvInkassoÄrendeKölagt message)
        {
            await ValueTask.CompletedTask;
            _logger.LogInformation("Ärende kölagt: {msg}", message);
        }
    }
}
