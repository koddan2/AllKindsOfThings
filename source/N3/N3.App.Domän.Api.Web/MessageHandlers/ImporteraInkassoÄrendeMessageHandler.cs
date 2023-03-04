using Marten;
using N3.App.Domän.Api.Web.Messages;
using N3.CqrsEs.Messages;
using N3.CqrsEs.SkrivModell.JobbPaket;
using Rebus.Bus;
using Rebus.Handlers;

namespace N3.App.Domän.Api.Web.MessageHandlers
{
    public class ImporteraInkassoÄrendeMessageHandler
        : IHandleMessages<ImporteraInkassoÄrendeJobbKommando>
    {
        private readonly ILogger<ImporteraInkassoÄrendeMessageHandler> _logger;
        private readonly IDocumentStore _store;
        private readonly IBus _bus;

        public ImporteraInkassoÄrendeMessageHandler(
            ILogger<ImporteraInkassoÄrendeMessageHandler> logger,
            IDocumentStore store,
            IBus bus
        )
        {
            _logger = logger;
            _store = store;
            _bus = bus;
        }

        public async Task Handle(ImporteraInkassoÄrendeJobbKommando message)
        {
            using var session = _store.LightweightSession();
            var existing = await session
                .Query<ImporteraInkassoÄrendeJobb>()
                .FirstOrDefaultAsync(x => x.Id == message.Jobb.Id);
            if (existing is null)
            {
                session.Store(message);
                await session.SaveChangesAsync();
                var msg = new ImportAvInkassoÄrendeKölagt { JobbId = message.Jobb.Id };
                _logger.LogInformation("Publishing: {msg}", msg);
                await _bus.Publish(msg);
            }
        }
    }
}
